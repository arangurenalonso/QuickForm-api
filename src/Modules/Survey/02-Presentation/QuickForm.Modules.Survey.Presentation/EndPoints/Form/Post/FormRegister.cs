using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class FormRegister : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("form/register", async (FormRegisterRequest request, ISender sender) =>
        {
            var result = await sender.Send(new FormRegisterCommand(
                request.Name,
                request.Description
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithName("Form.FormRegister")
        .WithTags(Tags.Form);
    }

    internal sealed class FormRegisterRequest
    {
        public string Name { get; init; }

        public string? Description { get; init; }
    }
}
