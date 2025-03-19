using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace QuickForm.Common.Infrastructure;
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ?
        parsedUserId :
            throw new ApplicationException("User identifier is unavailable");
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {
        IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
                                              throw new ApplicationException("Permissions are unavailable");

        return permissionClaims.Select(c => c.Value).ToHashSet();
    }
}
