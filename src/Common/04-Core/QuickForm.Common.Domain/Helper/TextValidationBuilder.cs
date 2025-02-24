using System.Text.RegularExpressions;

namespace QuickForm.Common.Domain;
public sealed class TextValidationBuilder
{
    private readonly List<string> _rules = new();
    private readonly List<string> _validCharsDescriptions = new();

    public TextValidationBuilder AddLowercaseLetters()
    {
        _rules.Add("a-z");
        _validCharsDescriptions.Add("lowercase letters");
        return this;
    }

    public TextValidationBuilder AddUppercaseLetters()
    {
        _rules.Add("A-Z");
        _validCharsDescriptions.Add("uppercase letters");
        return this;
    }

    public TextValidationBuilder AddLowercaseAccentedLetters()
    {
        _rules.Add("áéíóú");
        _validCharsDescriptions.Add("lowercase letters with accents");
        return this;
    }

    public TextValidationBuilder AddUppercaseAccentedLetters()
    {
        _rules.Add("ÁÉÍÓÚ");
        _validCharsDescriptions.Add("uppercase letters with accents");
        return this;
    }

    public TextValidationBuilder AddSpanishLetters()
    {
        _rules.Add("ñÑ");
        _validCharsDescriptions.Add("Spanish letters (ñ, Ñ)");
        return this;
    }

    public TextValidationBuilder AddNumbers()
    {
        _rules.Add("0-9");
        _validCharsDescriptions.Add("numbers");
        return this;
    }

    public TextValidationBuilder AddWhitespace()
    {
        _rules.Add("\\s");
        _validCharsDescriptions.Add("spaces");
        return this;
    }

    public TextValidationBuilder AddPunctuation()
    {
        _rules.Add("\\.,'!?¡¿;:");
        _validCharsDescriptions.Add("punctuation marks");
        return this;
    }

    public TextValidationBuilder AddSpecialChars()
    {
        _rules.Add("@#$%&*\\-");
        _validCharsDescriptions.Add("special characters (@, #, $, %, &, *, -)");
        return this;
    }
    public TextValidationBuilder AddAlphabeticCharacters()
    {
        return AddLowercaseLetters()
               .AddUppercaseLetters()
               .AddLowercaseAccentedLetters()
               .AddUppercaseAccentedLetters()
               .AddSpanishLetters();
    }
    public TextValidationBuilder AddAlphanumericCharacters()
    {
        return AddAlphabeticCharacters()
               .AddNumbers();
    }
    public TextValidationBuilder AddUnderscore()
    {
        _rules.Add("_");
        _validCharsDescriptions.Add("underscore (_)");
        return this;
    }
    public TextValidationBuilder AddHyphen()
    {
        _rules.Add("-");
        _validCharsDescriptions.Add("hyphen (-)");
        return this;
    }


    public TextValidation Build()
    {
        var pattern = $"^[{string.Join("", _rules)}]+$";
        var regexPattern = new Regex(pattern);
        return TextValidation.Create(_rules, regexPattern);
    }
}
