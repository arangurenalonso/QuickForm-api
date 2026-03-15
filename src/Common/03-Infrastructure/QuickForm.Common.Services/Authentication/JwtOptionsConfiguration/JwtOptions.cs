
namespace QuickForm.Common.Infrastructure;
public sealed class JwtOptions
{
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationTimeInMinutes { get; set; } = 15;
}
