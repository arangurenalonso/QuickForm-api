using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class ResendEmailConfirmation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/resend-email-confirmation", async (RequestResendEmailConfirmation request, ISender sender) =>
        {
            var result = await sender.Send(new ResendEmailConfirmationCommand(
                request.Email
                ));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting(Tags.Auth)
        .WithName("Auth.ResendEmailConfirmation")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestResendEmailConfirmation
    {
        public required string Email { get; init; }
    }
}
