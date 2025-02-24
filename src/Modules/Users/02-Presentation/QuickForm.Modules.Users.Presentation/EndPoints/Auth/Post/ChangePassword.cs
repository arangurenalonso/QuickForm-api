using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class ChangePassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/change-password", async (RequestChangePassword request, ISender sender) =>
        {
            var result = await sender.Send(new ChangePasswordCommand(
                request.CurrentPassword,
                request.NewPassword
                ));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Auth);
    }

    internal sealed class RequestChangePassword
    {
        public string CurrentPassword { get; init; }
        public string NewPassword { get; init; }
    }
}
