using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public sealed record NameVO
{

    public string Value { get; }

    private NameVO(string value)
    {
        Value = value;
    }

    public NameVO()
    {
    }

    public static ResultT<NameVO> Create(string? name)
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

        return new NameVO(name);
    }
    public static implicit operator string(NameVO name) => name.Value;
}
