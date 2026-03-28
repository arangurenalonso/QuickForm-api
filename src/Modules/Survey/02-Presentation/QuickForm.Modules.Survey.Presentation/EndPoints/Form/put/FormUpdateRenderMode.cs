using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class FormUpdateRenderMode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("form/{id}/render-mode", async (Guid id, FormUpdateRenderModeRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateRenderModeCommand(
                id,
                request.IdTypeRender));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Form.UpdateRenderMode")
        .WithTags(Tags.Form);
    }

    internal sealed class FormUpdateRenderModeRequest
    {
        public Guid IdTypeRender { get; init; }
    }
}
