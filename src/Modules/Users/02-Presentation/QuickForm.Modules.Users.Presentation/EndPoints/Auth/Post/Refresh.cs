using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class Refresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh", async (RequestRefresh request, ISender sender) =>
        {
            var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting("auth")
        .WithName("Auth.Refresh")
        .WithTags(Tags.Auth)
        .Produces<AuthSessionResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    internal sealed class RequestRefresh
    {
        public required string RefreshToken { get; init; }
    }
}
