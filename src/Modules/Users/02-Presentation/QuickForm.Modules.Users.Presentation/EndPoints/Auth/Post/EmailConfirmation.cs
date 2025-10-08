using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class EmailConfirmation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/confirm-email", async (RequestEmailConfirmation request, ISender sender) =>
        {
            var result = await sender.Send(new EmailConfirmationCommand(
                request.Token));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Auth.EmailConfirmation")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestEmailConfirmation
    {
        public string Token { get; init; }
    }
}
