using Microsoft.AspNetCore.Authorization;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Authorization;

namespace QuickForm.Common.Infrastructure;
internal sealed class PermissionRequirementHandler(
    IPermissionService _permissionService,
    ICurrentUserService _currentUserService
    ) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
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

}
