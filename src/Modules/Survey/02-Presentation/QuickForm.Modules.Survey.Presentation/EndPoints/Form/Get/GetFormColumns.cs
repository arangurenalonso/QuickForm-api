using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetFormColumns : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/columns",
            async (ISender sender, CancellationToken ct = default) =>
            {
                var result = await sender.Send(
                    new GetFormColumnsQuery(),
                    ct
                );

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            //.RequireAuthorization()
            .WithName("Form.FormColumns")
            .WithTags(Tags.Form);
    }
}
