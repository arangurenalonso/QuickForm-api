using QuickForm.Common.Presentation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Presentation;

internal sealed class ExportFormSubmissionsExcel : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("me/form/{idForm}/submissions/export-excel",
            async (ISender sender, Guid idForm, CancellationToken ct) =>
            {
                var result = await sender.Send(new ExportFormSubmissionsExcelCommand(idForm), ct);

                return result.Match(
                    file => Results.File(
                        file.Content,
                        file.ContentType,
                        file.FileName
                    ),
                    ApiResults.Problem
                );
            })
            .RequireAuthorization()
            .WithName("Form.ExportSubmissionsExcel")
            .WithTags(Tags.Form);
    }
}
