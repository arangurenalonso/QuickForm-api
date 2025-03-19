using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace QuickForm.Common.Infrastructure;
/// <summary>
/// Proveedor de políticas de autorización dinámicas en ASP.NET Core.
///
/// En ASP.NET Core, las políticas de autorización se definen manualmente en `Program.cs` usando `AddAuthorization()`.
/// Sin embargo, cuando los permisos son dinámicos (por ejemplo, almacenados en una base de datos),
/// definir cada política manualmente puede ser poco práctico.
///
/// Esta clase **genera automáticamente** una política de autorización cuando un endpoint solicita una política
/// que aún no ha sido definida. En lugar de requerir manualmente un claim en `AddPolicy()`, esta implementación
/// delega la validación a un `AuthorizationHandler`, permitiendo un control más flexible y escalable.
///
/// **Cómo sería de forma manual en `Program.cs`**
///
/// builder.Services.AddAuthorization(options =>
/// {
///     options.AddPolicy("users:read", policy =>
///     {
///         policy.RequireAuthenticatedUser();
///         policy.RequireClaim("Permission", "users:read");
///     });
/// });
/// ```
/// Cómo funciona esta implementación (dinámica):
/// 1️) Cuando un endpoint usa `.RequireAuthorization("users:read")`, ASP.NET busca la política `"users:read"`.
/// 2️) Si la política no existe, `PermissionAuthorizationPolicyProvider` la genera automáticamente.
/// 3️) Se crea una política con un `PermissionRequirement("users:read")`.
/// 4️) El `PermissionRequirementHandler` evalúa si el usuario tiene el claim `"Permission"` con el valor `"users:read"`.
/// 5️) Si el usuario tiene el permiso, se le concede acceso; si no, recibe `403 Forbidden`.
///
/// Ventajas de este enfoque:
/// - No es necesario definir manualmente cada política en `Program.cs`.
/// - Permite cargar permisos desde una base de datos de manera dinámica.
/// - Es más flexible y escalable para aplicaciones con múltiples permisos y roles.
///
/// </summary>
internal sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _authorizationOptions = options.Value;
    }

    // Método que devuelve la política de autorización basada en el nombre de la política solicitada.
    // Si la política no ha sido definida manualmente en AuthorizationOptions, se genera dinámicamente.
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // ASP.NET Core intenta buscar la política en las opciones de autorización existentes.
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
        // Si la política ya existe, se devuelve sin necesidad de crearla.
        if (policy is not null)
        {
            return policy;
        }
        // Como la política no existe, PermissionAuthorizationPolicyProvider la genera dinámicamente.
        // Se crea una nueva política de autorización con un PermissionRequirement asociado al nombre de la política.

        AuthorizationPolicy permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName)) // Agrega el requerimiento personalizado
            .Build();
        // Se almacena la nueva política en _authorizationOptions para futuras consultas,
        // evitando que se cree nuevamente la próxima vez que sea requerida.
        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        // Validación del permiso:
        // La política generada incluye un PermissionRequirement, pero NO busca el claim directamente aquí.
        // En su lugar, delega la validación al PermissionRequirementHandler, que evaluará si el usuario
        // tiene el claim "Permission" con el valor correspondiente, como "users:read".


        return permissionPolicy;
    }
}
