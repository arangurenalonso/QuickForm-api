using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class SubmitForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("form/{idForm}/submit", async (Dictionary<string, JsonElement> request, Guid idForm, ISender sender) =>
        {
            var result = await sender.Send(new CreateSubmissionCommand(
                idForm,
                request
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithName("Form.Submit")
        .WithTags(Tags.Form);
    }
}
