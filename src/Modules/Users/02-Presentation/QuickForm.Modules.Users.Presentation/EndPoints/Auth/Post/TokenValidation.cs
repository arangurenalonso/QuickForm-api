using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class TokenValidation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/token-validation", async (RequestTokenValidation request, ISender sender) =>
        {
            var result = await sender.Send(new TokenValidationCommand(
                request.Token
                ));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestTokenValidation
    {
        public string Token { get; init; }
    }
}
