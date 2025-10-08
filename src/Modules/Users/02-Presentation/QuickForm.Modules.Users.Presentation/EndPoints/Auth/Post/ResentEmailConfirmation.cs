using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class ResentEmailConfirmation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/resent-email-confirmation", async (RequestResentEmailConfirmation request, ISender sender) =>
        {
            var result = await sender.Send(new ResentEmailConfirmationCommand(
                request.Email
                ));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Auth.ResentEmailConfirmation")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestResentEmailConfirmation
    {
        public string Email { get; init; }
    }
}
