using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class ResetPassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/reset-password", async (RequestResetPassword request, ISender sender) =>
        {
            var result = await sender.Send(new ResetPasswordCommand(
                request.Email,
                request.VerificationCode,
                request.Password,
                request.ConfirmPassword
                ));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Auth.ResetPassword")
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestResetPassword
    {
        public string Email { get; init; }
        public string VerificationCode { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
    }
}
