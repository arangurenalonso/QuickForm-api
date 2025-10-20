using Microsoft.AspNetCore.Diagnostics;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Presentation;
namespace QuickForm.Api;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");
        var listResultError = CommonMethods.ConvertExceptionToResult(exception, "Unhandled exception occurred");

        var response = new ResultResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error - Unspecified Error",
            Errors = listResultError
        };
        
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
