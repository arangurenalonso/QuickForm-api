using MediatR;
using Microsoft.Extensions.Logging;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Domain;
using System;
using FluentValidation.Results;
using System.Reflection;

namespace QuickForm.Common.Application;
internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception for {RequestName}", typeof(TRequest).Name);
            List<ResultError> errors = CommonMethods.ConvertExceptionToResult(e, "UncontrollableError");

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultT<>))
            {
                Type resultType = typeof(TResponse).GetGenericArguments()[0];

                MethodInfo? failureMethod = typeof(ResultT<>)
                    .MakeGenericType(resultType)
                    .GetMethod(nameof(ResultT<object>.FailureTListResultError));

                if (failureMethod is not null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { ResultType.UnexpectedError, errors });
                }
            }
            return (TResponse)(object)Result.Failure(errors);
        }
    }
}
