using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class GetMyFormsPagination : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("me/forms/search", async (
                ISender sender,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromBody] List<FiltersForm>? filters = null
            ) =>
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var result = await sender.Send(
                new GetMyFormsPaginationQuery(filters, page, pageSize)
            );
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithName("Form.GetMyFormsPagination")
        .WithTags(Tags.Form);
    }
}

