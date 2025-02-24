using System.Security.Claims;

namespace QuickForm.Common.Infrastructure;
public static class ClaimsPrincipalExtension
{
    public static string? GetClaim(this ClaimsPrincipal principal, string name)
    {
        var claim = principal.Claims.FirstOrDefault(c => c.Type == name);
        return claim?.Value;
    }

}
