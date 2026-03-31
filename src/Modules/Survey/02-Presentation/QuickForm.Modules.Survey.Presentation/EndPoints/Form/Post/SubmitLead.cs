using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class SubmitLead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("submit-lead", async (RegisterLeadCommand request, ISender sender) =>
        {
            var result = await sender.Send(new RegisterLeadCommand(
                request.Name,
                request.Email,
                request.PhoneNumber
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        //.RequireAuthorization()
        .WithName("Form.SubmitLead")
        .WithTags(Tags.Form);
    }

    internal sealed class SubmitLeadRequest
    {
        public string Name { get; init; }

        public string Email { get; init; }
        public string PhoneNumber { get; init; }    
    }
}
