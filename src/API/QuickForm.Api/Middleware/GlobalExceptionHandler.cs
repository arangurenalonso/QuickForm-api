using Microsoft.AspNetCore.Diagnostics;
using QuickForm.Common.Domain.Method;

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

        await httpContext.Response.WriteAsJsonAsync(listResultError, cancellationToken);

        return true;
    }
}
