using System.Linq;
using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using QuickForm.Common.Domain;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Users.Application;

namespace QuickForm.Modules.Users.Presentation;

internal sealed class EndpointsExplorer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("_meta/endpoints",async (EndpointDataSource dataSource, ISender sender) =>
        {
            var routes = dataSource.Endpoints
                .OfType<RouteEndpoint>()
                .Select(e =>
                {
                    var md = e.Metadata;

                    var methods = md.GetMetadata<HttpMethodMetadata>()?.HttpMethods?.ToArray()
                                 ?? Array.Empty<string>();
                    var tags = md.GetMetadata<ITagsMetadata>()?.Tags?.ToArray()
                               ?? Array.Empty<string>();
                    var name = md.GetMetadata<IEndpointNameMetadata>()?.EndpointName;

                    var hasAuthorize = md.GetOrderedMetadata<IAuthorizeData>()?.Any() == true;
                    var hasAllowAnonymous = md.GetOrderedMetadata<IAllowAnonymous>()?.Any() == true;

                    var requiresAuthorization = !hasAllowAnonymous && hasAuthorize;

                    return new EndpointInfo(
                        Pattern: ToPattern(e.RoutePattern),
                        Methods: methods,
                        Name: name,
                        DisplayName: e.DisplayName,
                        Tags: tags,
                        RequiresAuthorization: requiresAuthorization
                    );
                })
                .OrderBy(r => r.Pattern)
                .ThenBy(r => string.Join(",", r.Methods))
                .ToList();

            var idApplication = AppConstants.ApplicationCode;
            var command = new RegisterPermissionCommand(routes, idApplication);


            var result = await sender.Send(command);
            Console.WriteLine(result);

            //return result.Match(Results.Ok, ApiResults.Problem)
            return Results.Ok(routes);
        })
        .RequireAuthorization()
        .WithTags(Tags.System)
        .WithName("ListEndpoints");
    }

    private static string ToPattern(RoutePattern rp)
    {
        if (!string.IsNullOrEmpty(rp.RawText))
        {
            return rp.RawText!;
        }

        var sb = new StringBuilder();
        foreach (var seg in rp.PathSegments)
        {
            sb.Append('/');
            foreach (var part in seg.Parts)
            {
                switch (part)
                {
                    case RoutePatternLiteralPart lit:
                        sb.Append(lit.Content);
                        break;

                    case RoutePatternParameterPart par:
                        sb.Append('{');
                        if (par.IsCatchAll)
                        {
                            sb.Append('*'); // {*slug}
                        }
                        sb.Append(par.Name);

                        if (par.ParameterPolicies.Count > 0)
                        {
                            sb.Append(':');
                            sb.Append(string.Join(":", par.ParameterPolicies.Select(p => p.Content)));
                        }
                        if (par.Default is not null)
                        {
                            sb.Append('=').Append(par.Default);
                        }
                        if (par.IsOptional)
                        {
                            sb.Append('?');
                        }
                        sb.Append('}');
                        break;
                }
            }
        }
        return sb.Length == 0 ? "/" : sb.ToString();
    }

}
