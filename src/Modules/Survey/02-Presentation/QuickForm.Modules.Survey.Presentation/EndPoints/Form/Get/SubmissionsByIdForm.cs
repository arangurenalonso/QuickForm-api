using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class SubmissionsByIdForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/{formId:guid}/submissions",
            async (ISender sender, 
                    Guid formId,
                    [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 10,
                    CancellationToken ct=default) =>
            {
                page = page < 1 ? 1 : page;
                pageSize = pageSize < 1 ? 10 : pageSize;

                var result = await sender.Send(
                    new GetFormSubmissionsQuery(formId, page, pageSize),
                    ct
                );

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            //.RequireAuthorization()
            .WithName("Form.FormSubmissions")
            .WithTags(Tags.Form);
    }
}
