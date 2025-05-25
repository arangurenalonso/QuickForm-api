﻿using QuickForm.Common.Presentation;
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
                    x.Question.Select(q => new QuestionDto(
                            q.Id,
                            q.Type,
                            q.Properties
                        )).ToList()
                )).ToList();
            var result = await sender.Send(new SaveFormStructureCommand(
                idForm,
                sectionsDto
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Form);
    }

    internal sealed class FormSectionRegisterDtoRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<FormQuestionRegisterDtoRequest> Question { get; set; }
    }

    internal sealed class FormQuestionRegisterDtoRequest
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;

        // Usamos JsonElement para representar datos dinámicos (más eficiente que Dictionary<string, object>)
        public JsonElement Properties { get; set; }
    }
}
