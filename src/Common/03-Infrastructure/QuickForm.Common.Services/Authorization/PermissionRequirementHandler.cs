using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Authorization;

namespace QuickForm.Common.Infrastructure;
internal sealed class PermissionRequirementHandler(
    IPermissionService _permissionService,
    IHttpContextAccessor _httpContextAccessor,
    ICurrentUserService _currentUserService
    ) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {

        var http = _httpContextAccessor.HttpContext;
        var endpoint = http?.GetEndpoint();                             // Endpoint
        var routeEndpoint = endpoint as RouteEndpoint;

        var endpointName = endpoint?.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName;
        var displayName = endpoint?.DisplayName;
        var pattern = routeEndpoint is not null ? ToPattern(routeEndpoint.RoutePattern) : http?.Request.Path.Value ?? "";
        var methods = endpoint?.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods?.ToArray() ?? [];
        var tags = endpoint?.Metadata.GetMetadata<ITagsMetadata>()?.Tags?.ToArray() ?? [];

        var routeValues = http?.GetRouteData()?.Values;
        var permissionSource =
            !string.IsNullOrWhiteSpace(endpointName)
                ? endpointName!                                    // p.ej. "Person.Country.Register.v1"
                : $"{tags.FirstOrDefault() ?? "Unknown"}:{methods.FirstOrDefault() ?? http?.Request.Method ?? "GET"}:{pattern}";

        Console.WriteLine(displayName);
        Console.WriteLine(routeValues);
        Console.WriteLine(permissionSource);


        ResultT< (string Left, string Right)> result = ValidateAndSplitPermission(requirement.Permission);

        var userIdResult = _currentUserService.UserId;
        if (result.IsSuccess && userIdResult.IsSuccess)
        {

            (string resource, string action) = result.Value;
            var idUser= userIdResult.Value;
            bool hasPermission = await _permissionService.HasPermissionAsync(idUser, resource, action);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
    public ResultT<(string Left, string Right)> ValidateAndSplitPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return ResultError.NullValue("Permission","Permission cannot be null or empty.");
        }

        var parts = permission.Split(':');

        if (parts.Length != 2)
        {
            return ResultError.InvalidFormat("Permission", "Invalid permission format. It must be in the format 'Resource:Action'.");
        }

        return (parts[0], parts[1]);
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
                            sb.Append('*');
                        }
                        sb.Append(par.Name);
                        if (par.ParameterPolicies.Count > 0)
                        {
                            sb.Append(':').Append(string.Join(":", par.ParameterPolicies.Select(p => p.Content)));
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
