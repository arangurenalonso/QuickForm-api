﻿using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetFormStructureByIdForm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("form/{idForm}/structure", async (Guid idForm, ISender sender) =>
        {
            var result = await sender.Send(new GetFormStructureQuery(
                    idForm
                ));
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Form);
    }
}
