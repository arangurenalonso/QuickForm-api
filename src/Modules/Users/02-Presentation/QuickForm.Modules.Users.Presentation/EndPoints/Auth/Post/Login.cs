using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (RequestLogin request, ISender sender) =>
        {
            var result = await sender.Send(new LoginCommand(
                request.Email,
                request.Password));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting(Tags.Auth)
        .WithName("Auth.Login")
        .WithTags(Tags.Auth)
        .WithSummary("Login user")
        .WithDescription("Authenticates the user and returns an access token.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    internal sealed class RequestLogin
    {
        public required string  Email { get; init; }

        public required string Password { get; init; }
    }
}
