using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (RequestRegister request, ISender sender) =>
        {
            var result = await sender.Send(new RegisterCommand(
                request.Email,
                request.Password,
                request.ConfirmPassword));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting(Tags.Auth)
        .WithName("Auth.RegisterUser")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestRegister
    {
        public required string Email { get; init; } 
        public required string Password { get; init; }
        public required string ConfirmPassword { get; init; }
    }
}
