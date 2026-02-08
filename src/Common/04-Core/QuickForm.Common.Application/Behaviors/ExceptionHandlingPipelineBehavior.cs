using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickForm.Common.Application;
internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
        ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger
    ): IPipelineBehavior<TRequest, TResponse>
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
            return ResultHelper.CreateFailureResponse<TResponse>(ResultType.UnexpectedError, errors);
        }
    }
}
