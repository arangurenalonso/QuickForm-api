
namespace QuickForm.Modules.Users.Options;
public class JwtOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SecretKey { get; init; }
    public string Subject { get; init; }
    public int ExpirationTimeInHours { get; init; }

}
