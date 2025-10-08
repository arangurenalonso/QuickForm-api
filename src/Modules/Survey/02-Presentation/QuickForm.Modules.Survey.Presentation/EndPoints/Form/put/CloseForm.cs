using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class CloseForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("form/{id}/close", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new FormCloseCommand(
                id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Form.CloseForm")
        .WithTags(Tags.Form);
    }
}
