using FluentValidation.Results;
using FluentValidation;
using MediatR;
using QuickForm.Common.Domain;
using System.Reflection;

namespace QuickForm.Common.Application;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[]  validationFailures = await ValidateAsync(request);
        if (validationFailures.Length == 0)
        {
            return await next();
        }

        List<ResultError> errors = validationFailures.Select(validationFailure => 
                                                    ResultError.InvalidInput(
                                                        validationFailure.PropertyName,
                                                        validationFailure.ErrorMessage
                                                        )
                                                    ).ToList();
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultT<>))
        {
            Type resultType = typeof(TResponse).GetGenericArguments()[0];

            MethodInfo? failureMethod = typeof(ResultT<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(ResultT<object>.FailureTListResultError));

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { ResultType.FluentValidation,errors });
            }
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errors);
        }

        throw new ValidationException(validationFailures);
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

}
