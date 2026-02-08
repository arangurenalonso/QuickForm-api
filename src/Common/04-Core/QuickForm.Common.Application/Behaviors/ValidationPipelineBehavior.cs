using FluentValidation.Results;
using FluentValidation;
using MediatR;
using QuickForm.Common.Domain;
using System.Reflection;

namespace QuickForm.Common.Application;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators
    ): IPipelineBehavior<TRequest, TResponse>
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

        return ResultHelper.CreateFailureResponse<TResponse>(ResultType.FluentValidation, errors);
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
