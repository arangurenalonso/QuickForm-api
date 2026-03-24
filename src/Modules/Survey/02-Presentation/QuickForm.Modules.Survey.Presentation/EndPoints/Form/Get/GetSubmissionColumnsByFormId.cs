using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetSubmissionColumnsByFormId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/{formId:guid}/submissions/columns",
            async (ISender sender, Guid formId, CancellationToken ct = default) =>
            {
                var result = await sender.Send(
                    new GetFormSubmissionColumnsQuery(formId),
                    ct
                );

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            //.RequireAuthorization()
            .WithName("Form.SubmissionColumns")
            .WithTags(Tags.Form);
    }
}
