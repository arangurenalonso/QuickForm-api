using System.Text.RegularExpressions;

namespace QuickForm.Common.Domain;

public sealed class TextValidation
{
    private readonly IReadOnlyList<string> _validDescriptions;
    private readonly Regex _fullRegex;
    private readonly Regex _singleCharRegex;

    private TextValidation(IReadOnlyList<string> validDescriptions, Regex fullRegex, Regex singleCharRegex)
    {
        _validDescriptions = validDescriptions;
        _fullRegex = fullRegex;
        _singleCharRegex = singleCharRegex;
    }

    public static TextValidation Create(
        IReadOnlyList<string> validDescriptions,
        Regex fullRegex,
        Regex singleCharRegex
    )
        => new TextValidation(validDescriptions, fullRegex, singleCharRegex);

    public Result ValidateInvalidCharacter(string field, string value)
    {
        var valueText = value.Trim();

        if (_fullRegex.IsMatch(valueText))
        {
            return Result.Success();
        }

        var invalidChars = valueText
            .Where(c => !_singleCharRegex.IsMatch(c.ToString()))
            .Distinct()
            .Select(c => c == ' ' ? "space" : c.ToString())
            .ToList();

        var allowed = string.Join(", ", _validDescriptions);

        var errorMessage =
            $"The field contains invalid characters [{string.Join(", ", invalidChars)}]. " +
            $"Allowed: {allowed}.";

        return ResultError.InvalidCharacter(field, errorMessage);
    }
}
