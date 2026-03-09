using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class QuestionTypeFiltersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("question-type/filters",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAllQuestionTypeFiltersQuery(), ct);
                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithName("QuestionType.GetAllFilters")
            .WithTags(Tags.Form);
    }
}
