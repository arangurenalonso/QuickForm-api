using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record ValidationMessageTemplateVO
{
    private static readonly Regex PlaceholderRegex =
        new(@"\{[a-zA-Z_][a-zA-Z0-9_]*\}", RegexOptions.Compiled);

    public string ValidationMessage { get; }
    public string? PlaceholderKey { get; }

    private ValidationMessageTemplateVO(string message, string? placeholderKey)
    {
        ValidationMessage = message;
        PlaceholderKey = placeholderKey;
    }

    public static ResultT<ValidationMessageTemplateVO> Create(
        string defaultValidationMessage,
        string? placeholderKey = null
    )
    {
        if (string.IsNullOrWhiteSpace(defaultValidationMessage))
        {
            return ResultError.EmptyValue(
                "DefaultValidationMessage",
                "Default validation message cannot be empty."
            );
        }

        var message = defaultValidationMessage.Trim();

        var found = PlaceholderRegex.Matches(message)
            .Select(m => m.Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (placeholderKey is null)
        {
            return new ValidationMessageTemplateVO(message, null);
        }

        var placeholder = placeholderKey.Trim();

        if (!PlaceholderRegex.IsMatch(placeholder))
        {
            return ResultError.InvalidFormat(
                "Placeholder",
                $"Placeholder '{placeholder}' is invalid. Use format like '{{min}}'."
            );
        }

        if (!found.Contains(placeholder))
        {
            return ResultError.InvalidInput(
                "Placeholder",
                $"Placeholder '{placeholder}' must exist in DefaultValidationMessage."
            );
        }

        return new ValidationMessageTemplateVO(message, placeholder);
    }

}
