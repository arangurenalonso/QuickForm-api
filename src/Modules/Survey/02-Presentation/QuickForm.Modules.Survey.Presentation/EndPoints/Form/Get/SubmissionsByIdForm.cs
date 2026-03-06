using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class SubmissionsByIdForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/{formId:guid}/submissions",
            async (ISender sender, Guid formId, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetFormSubmissionsQuery(formId), ct);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            //.RequireAuthorization()
            .WithName("Form.FormSubmissions")
            .WithTags(Tags.Form);
    }
}
