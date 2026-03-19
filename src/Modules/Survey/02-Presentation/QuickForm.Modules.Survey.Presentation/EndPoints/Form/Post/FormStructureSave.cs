using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class FormStructureSave : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("form/{idForm}/structure", async (
            Guid idForm,
            List<FormSectionRegisterDtoRequest> request,
            ISender sender) =>
        {
            var sectionsDto =
                request.Select(x => new SectionDto(
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
            var result = await sender.Send(new SaveFormStructureCommand(
                idForm,
                sectionsDto
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithName("Form.FormStructureSave")
        .WithTags(Tags.Form);
    }

   
}
