using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record TokenVO
{
    public string Value { get; }

    private TokenVO(string value)
    {
        Value = value;
    }

    public static TokenVO Restore(string value) => new TokenVO(value);
    public static TokenVO GenerateOtp()
    {
        var token = AuthTokenGenerator.GenerateNumericToken(6);
        return new TokenVO(token);
    }

    public static TokenVO GenerateLongToken(int length = 32)
    {
        var token = AuthTokenGenerator.GenerateAlphanumericToken(length);
        return new TokenVO(token);
    }

    public static implicit operator string(TokenVO token) => token.Value;
}
