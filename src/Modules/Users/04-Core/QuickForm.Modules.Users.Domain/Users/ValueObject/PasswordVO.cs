using QuickForm.Common.Domain;
using System.Text.RegularExpressions;

namespace QuickForm.Modules.Users.Domain;
public sealed record PasswordVO 
{
    public string Value { get; }

    private static readonly Regex passwordLength = new Regex(@".{8,}", RegexOptions.Compiled);
    private static readonly Regex passwordLowercase = new Regex(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex passwordUppercase = new Regex(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex passwordNumber = new Regex(@"\d", RegexOptions.Compiled);
    private static readonly Regex passwordSpecialChar = new Regex(@"[@#$%&*\\-]", RegexOptions.Compiled);

    private PasswordVO(string value)
    {
        Value = value;
    }

    public PasswordVO()
    {
    }

    public static ResultT<PasswordVO> Create(string? password,
        IPasswordHashingService? passwordHashingService = null)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ResultError.EmptyValue("Password", "Password cannot be null or empty.");
        }
        if (passwordHashingService is null)
        {
            return new PasswordVO(password);
        }

        var reasons = ValidatePassword(password);

        if (reasons.Count > 0)
        {
            var errorList = reasons.Select(reason => ResultError.InvalidFormat("Password", reason)).ToList();
            return new ResultErrorList(errorList);

        }
        var passwordHash = passwordHashingService.HashPassword(password);
        return new PasswordVO(passwordHash);
    }

    private static List<string> ValidatePassword(string value)
    {
        var reasons = new List<string>();

        if (!passwordLength.IsMatch(value))
        {
            reasons.Add("must be at least 8 characters long.");
        }
        if (!passwordLowercase.IsMatch(value))
        {
            reasons.Add("must contain at least one lowercase letter.");
        }
        if (!passwordUppercase.IsMatch(value))
        {
            reasons.Add("contain at least one uppercase letter.");
        }
        if (!passwordNumber.IsMatch(value))
        {
            reasons.Add("must contain at least one digit.");
        }
        if (!passwordSpecialChar.IsMatch(value))
        {
            reasons.Add("must contain at least one special character (@, #, $, %, &, *, -).");
        }

        return reasons;
    }

    public static implicit operator string(PasswordVO password) => password.Value;
}
