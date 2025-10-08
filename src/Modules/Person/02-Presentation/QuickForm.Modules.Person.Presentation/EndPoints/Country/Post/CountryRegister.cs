using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Person.Application;

namespace QuickForm.Modules.Person.Presentation;

internal sealed class CountryRegister : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("master/country", async (CountryRegisterRequest request, ISender sender) =>
        {
            var result = await sender.Send(new RegisterCountryCommand(
                request.Name,
                request.Description
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithName("Common.RegisterCountry")
        .WithTags(Tags.Person);
    }

    internal sealed class CountryRegisterRequest
    {
        public string Name { get; init; }

        public string? Description { get; init; }
    }
}
