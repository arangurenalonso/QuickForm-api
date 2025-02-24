using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class FormUpdate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("form/{id}", async (Guid id, FormUpdateRequest request, ISender sender) =>
        {
            var result = await sender.Send(new FormUpdateCommand(
                id,
                request.Name,
                request.Description));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Form);
    }

    internal sealed class FormUpdateRequest
    {
        public string Name { get; init; }

        public string? Description { get; init; }
    }
}
