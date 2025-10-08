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
        .WithName("Auth.Login")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestLogin
    {
        public string Email { get; init; }

        public string Password { get; init; }
    }
}
