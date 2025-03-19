using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace QuickForm.Common.Infrastructure;
internal static class AuthorizationExtensions
{
    internal static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
