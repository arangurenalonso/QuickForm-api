using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record TokenVO
{
    private static readonly Regex OtpRegex = new(@"^\d{6}$", RegexOptions.Compiled);
    private static readonly Regex AlphaNumericRegex = new(@"^[A-Z0-9]{32}$", RegexOptions.Compiled);

    public string Value { get; }

    private TokenVO(string value)
    {
        Value = value;
    }

    public static ResultT<TokenVO> CreateOtp(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
        }

        var normalized = token.Trim();

        if (!OtpRegex.IsMatch(normalized))
        {
            return ResultError.InvalidFormat("Token", "OTP token must contain exactly 6 digits.");
        }

        return new TokenVO(normalized);
    }

    public static ResultT<TokenVO> CreateLongToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
        }

        var normalized = token.Trim().ToUpperInvariant();

        if (!AlphaNumericRegex.IsMatch(normalized))
        {
            return ResultError.InvalidFormat("Token", "Token must contain exactly 32 uppercase letters and digits.");
        }

        return new TokenVO(normalized);
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
