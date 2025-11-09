using System.Text.RegularExpressions;

namespace QuickForm.Common.Domain;
public class TextValidation
{
    private readonly List<string> _rules;
    private readonly Regex _regexPattern;

    private TextValidation(List<string> rules, Regex regexPattern)
    {
        _rules = rules;
        _regexPattern = regexPattern;
    }

    public static TextValidation Create(List<string> rules, Regex regexPattern)
    {
        return new TextValidation(rules, regexPattern);
    }

    public Result ValidateInvalidCharacter(string field, string value)
    {
        string valueText = value.Trim();
        var pattern = $"[{string.Join("", _rules)}]";
        var invalidChars = valueText
            .Where(c => !Regex.IsMatch(c.ToString(), pattern))
            .Distinct() 
            .ToList();

        if (_regexPattern == null)
        {
            throw new InvalidOperationException("Regex pattern not found; build before validating.");
        }

        if (!_regexPattern.IsMatch(valueText))
        {
            var errorMessage = $"The field contains invalid characters [{
                                        string.Join(",",
                                                    invalidChars.Select(x=>char.IsWhiteSpace(x) ? "whitespace" : x.ToString())
                                                    )
                                        }]";

            return ResultError.InvalidCharacter(field,errorMessage);
        }

        return Result.Success();
    }
}
