using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormNameVO
{

    public string Value { get; }

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
        var textValdiation = new TextValidationBuilder()
                                        .AddAlphabeticCharacters()
                                        .AddWhitespace()
                                        .Build().ValidateInvalidCharacter("Name", name);
        if (textValdiation.IsFailure)
        {
            return textValdiation.Errors;

        }

        return new FormNameVO(name);
    }
    public static implicit operator string(FormNameVO name) => name.Value;
}
