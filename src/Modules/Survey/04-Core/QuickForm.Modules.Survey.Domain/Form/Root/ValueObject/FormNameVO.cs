using System.Text.RegularExpressions;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormNameVO
{

    public string Value { get; private set; } = default!;

    private FormNameVO(string value)
    {
        Value = value;
    }
    public static ResultT<FormNameVO> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ResultError.EmptyValue("Name", "Name cannot be null or empty.");
        }

        var trimmed = NormalizeSpaces(name.Trim());

        const int maxLength = 255;
        if (trimmed.Length > maxLength)
        {
            return ResultError.InvalidFormat("Name", $"Name must be at most {maxLength} characters long.");
        }

        var validation = new TextValidationBuilder()
                                .AddUnicodeLetters()
                                .AddSpace()
                                .AddHyphen()
                                .AddNumbers()
                                .AddApostrophe()
                                .Build();

        var result = validation.ValidateInvalidCharacter("Name", trimmed);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return new FormNameVO(trimmed);
    }

    private static string NormalizeSpaces(string input)
        => Regex.Replace(input, @"[ ]{2,}", " ");
    public static implicit operator string(FormNameVO name) => name.Value;
}
