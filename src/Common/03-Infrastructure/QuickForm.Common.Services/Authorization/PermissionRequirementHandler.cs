using Microsoft.AspNetCore.Authorization;

namespace QuickForm.Common.Infrastructure;
internal sealed class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        HashSet<string> permissions = context.User.GetPermissions();
        // Busca si el usuario tiene el claim "Permission" con el valor del requerimiento

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);// Usuario autorizado
        }

        return Task.CompletedTask;  // Si no lo tiene, la autorización falla (403 Forbidden)
    }
}
