using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetFormSubmissionTemplate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/{idForm}/submission-template", async (Guid idForm, ISender sender) =>
        {
            var result = await sender.Send(new GetFormSubmissionTemplateQuery(
                    idForm
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        //.RequireAuthorization()
        .WithName("Submission.GetFormSubmissionTemplate")
        .WithTags(Tags.Submission);
    }
}
//slug_personalizado
//secure_hash
