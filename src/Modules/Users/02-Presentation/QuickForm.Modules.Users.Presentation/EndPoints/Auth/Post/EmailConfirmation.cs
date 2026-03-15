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
                request.Email,
                request.VerificationCode));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .RequireRateLimiting(Tags.Auth)
        .WithName("Auth.EmailConfirmation")
        .WithTags(Tags.Auth)
        .Produces<AuthSessionResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal sealed class RequestEmailConfirmation
    {
        public required string Email { get; set; }
        public required string VerificationCode { get; init; }
    }
}
