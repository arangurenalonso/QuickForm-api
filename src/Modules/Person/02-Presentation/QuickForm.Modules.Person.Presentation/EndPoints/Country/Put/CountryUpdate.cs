using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Person.Application;

namespace QuickForm.Modules.Person.Presentation;

internal sealed class CountryUpdate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("master/country/{id}", async (Guid id, CountryUpdateRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateCountryCommand(
                id,
                request.Name,
                request.Description
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Person);
    }

    internal sealed class CountryUpdateRequest
    {
        public string Name { get; init; }

        public string? Description { get; init; }
    }
}

