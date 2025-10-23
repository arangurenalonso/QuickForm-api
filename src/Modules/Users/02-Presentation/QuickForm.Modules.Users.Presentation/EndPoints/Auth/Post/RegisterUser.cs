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
        .WithName("Auth.RegisterUser")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestRegister
    {
        public string Email { get; init; }

        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
    }
}
