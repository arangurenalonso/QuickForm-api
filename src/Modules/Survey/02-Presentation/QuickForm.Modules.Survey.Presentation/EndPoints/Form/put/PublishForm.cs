using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class PublishForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("form/{id}/publish", async (Guid id,
                ISender sender,
                [FromBody] List<FormSectionRegisterDtoRequest> request
            ) =>
        {
            var sectionsDto =request.Select(x => new SectionDto(
                                                               x.Id,
                                                               x.Title,
                                                               x.Description,
                                                               x.Fields.Select(q => new QuestionDto(
                                                                       q.Id,
                                                                       q.Type,
                                                                       q.Properties,
                                                                       q.Rules?.ToDictionary(
                                                                           kvp => kvp.Key,
                                                                           kvp => new RuleDto(
                                                                                           kvp.Value.Value,
                                                                                           kvp.Value.MessageTemplate ?? string.Empty
                                                                                       )
                                                                       )
                                                                   )).ToList()
                                                           )).ToList();

            var result = await sender.Send(new FormPublishCommand(id, sectionsDto));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName("Form.PublishForm")
        .WithTags(Tags.Form);
    }
}
