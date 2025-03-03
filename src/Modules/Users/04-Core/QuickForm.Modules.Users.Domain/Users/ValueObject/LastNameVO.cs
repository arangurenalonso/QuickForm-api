using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record LastNameVO 
{

    public string Value { get; }

    private LastNameVO(string value)
    {
        Value = value;
    }
    public LastNameVO()
    {
    }
    public static ResultT<LastNameVO> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new LastNameVO();
        }
        var textValdiation = new TextValidationBuilder()
                                        .AddAlphabeticCharacters()
                                        .AddWhitespace()
                                        .Build().ValidateInvalidCharacter("LastName",name);
        if (textValdiation.IsFailure)
        {
            return textValdiation.Errors;

        }

        return new LastNameVO(name);
    }
    public static implicit operator string(LastNameVO name) => name.Value;
}
