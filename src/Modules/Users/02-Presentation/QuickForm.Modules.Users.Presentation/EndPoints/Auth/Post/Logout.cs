using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (RequestLogout request, ISender sender) =>
        {
            var result = await sender.Send(new LogoutCommand(request.RefreshToken));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting("auth")
        .WithName("Auth.Logout")
        .WithTags(Tags.Auth)
        .Produces<ResultResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal sealed class RequestLogout
    {
        public string RefreshToken { get; init; } = string.Empty;
    }
}
