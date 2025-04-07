using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;
using System.Text.Json;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class QuestionRegister : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("form/{idForm}/question", async (
            Guid idForm,
            List<FormQuestionRegisterRequest> request,
            ISender sender) =>
        {
            var result = await sender.Send(new FormQuestionRegisterCommand(
                idForm,
                request.Select(x => new QuestionDto(
                    x.Id,
                    x.Type,
                    x.Properties
                )).ToList()
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Form);
    }

    internal sealed class FormQuestionRegisterRequest
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;

        // Usamos JsonElement para representar datos dinámicos (más eficiente que Dictionary<string, object>)
        public JsonElement Properties { get; set; }
    }
}
