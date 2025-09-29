using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickForm.Common.Domain;
public sealed record KeyNameVO
{
    private static readonly Regex AllowedPattern =
        new(@"^[A-Z0-9](?:[A-Z0-9._-]{0,62}[A-Z0-9])?$", RegexOptions.Compiled); // 1..64

    public string Value { get; }

    private KeyNameVO(string value)
    {
        Value = value;
    }

    private KeyNameVO() { }

    public static ResultT<KeyNameVO> Create(string? keyName, int maxLength = 64)
    {
        if (string.IsNullOrWhiteSpace(keyName))
        {
            return ResultError.EmptyValue("KeyName", "KeyName cannot be null or empty.");
        }

        var normalized = NormalizeKey(keyName, maxLength);

        if (string.IsNullOrEmpty(normalized))
        {
            return ResultError.InvalidFormat("KeyName", "KeyName is invalid after normalization.");
        }

        if (!IsValid(normalized))
        {
            return ResultError.InvalidFormat("KeyName",
                "KeyName must match ^[A-Z0-9](?:[A-Z0-9._-]{0,62}[A-Z0-9])?$");

        }

        return new KeyNameVO(normalized);
    }


    public static implicit operator string(KeyNameVO key) => key.Value;

    private static bool IsValid(string input) => AllowedPattern.IsMatch(input);

    private static string NormalizeKey(string input, int maxLength)
    {
        var s = input.Trim();

        s = RemoveDiacritics(s);

        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (IsAlphaNumeric(ch) || ch == '.' || ch == '-' || ch == '_')
            {
                sb.Append(ch);
            }
            else if (!char.IsControl(ch))
            {
                sb.Append('_');
            }
        }

        s = sb.ToString();

        s = Regex.Replace(s, @"\s+", "_");

        s = Regex.Replace(s, @"[._-]{2,}", "_");

        s = s.Trim('.', '_', '-');

        s = s.ToUpperInvariant();

        if (s.Length > maxLength)
        {
            s = s.Substring(0, maxLength).Trim('.', '_', '-');
        }

        return s;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(capacity: normalized.Length);

        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static bool IsAlphaNumeric(char c)
    {
        if (c >= 'A' && c <= 'Z')
        {
            return true;
        }
        if (c >= 'a' && c <= 'z')
        {
            return true;
        }
        if (c >= '0' && c <= '9')
        {
            return true; 
        }
        return false;

    }
}
