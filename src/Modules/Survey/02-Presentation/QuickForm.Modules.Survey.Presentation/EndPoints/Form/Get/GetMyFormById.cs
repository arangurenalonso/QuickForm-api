using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetMyFormById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("me/form/{idForm}", async (ISender sender,Guid idForm) =>
        {
            var result = await sender.Send(new GetMyFormByIdQuery(idForm));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithName("Form.GetMyFormById")
        .WithTags(Tags.Form);
    }
}
