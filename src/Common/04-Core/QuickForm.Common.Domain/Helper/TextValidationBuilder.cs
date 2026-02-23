using System.Data;
using System.Text.RegularExpressions;

namespace QuickForm.Common.Domain;

public sealed class TextValidationBuilder
{
    private readonly List<string> _charClassParts = new();
    private readonly List<string> _validCharsDescriptions = new();

    public TextValidationBuilder AddUnicodeLetters()
    {
        // \p{L} = letters, \p{M} = combining marks (accents)
        _charClassParts.Add(@"\p{L}\p{M}");
        _validCharsDescriptions.Add("letters (including accents)");
        return this;
    }

    // Space only (not \s)
    public TextValidationBuilder AddSpace()
    {
        _charClassParts.Add(" ");
        _validCharsDescriptions.Add("spaces");
        return this;
    }

    public TextValidationBuilder AddNumbers()
    {
        _charClassParts.Add("0-9");
        _validCharsDescriptions.Add("numbers");
        return this;
    }

    public TextValidationBuilder AddApostrophe()
    {
        _charClassParts.Add("'");
        _validCharsDescriptions.Add("apostrophe (')");
        return this;
    }

    public TextValidationBuilder AddHyphen()
    {
        _charClassParts.Add(@"\-"); // safe inside [...]
        _validCharsDescriptions.Add("hyphen (-)");
        return this;
    }

    public TextValidationBuilder AddUnderscore()
    {
        _charClassParts.Add("_");
        _validCharsDescriptions.Add("underscore (_)");
        return this;
    }

    public TextValidation Build()
    {
        if (_charClassParts.Count == 0)
        {
            throw new InvalidOperationException("No rules configured.");
        }

        var charClass = string.Concat(_charClassParts);

        // Full string must be made only of allowed chars
        var fullPattern = $"^[{charClass}]+$";
        // Single character validator (for invalid char extraction)
        var charPattern = $"^[{charClass}]$";

        var options = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        return TextValidation.Create(
            _validCharsDescriptions,
            new Regex(fullPattern, options),
            new Regex(charPattern, options)
        );
    }
}
