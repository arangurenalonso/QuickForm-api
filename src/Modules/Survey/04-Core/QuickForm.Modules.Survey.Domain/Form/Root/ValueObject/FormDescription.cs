using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record FormDescription
{

    public string? Value { get; }

    private FormDescription(string? value=null)
    {
        Value = value;
    }

    public static ResultT<FormDescription> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new FormDescription();
        }
        var textValdiation = new TextValidationBuilder()
                                        .AddAlphabeticCharacters()
                                        .AddWhitespace()
                                        .Build().ValidateInvalidCharacter("Description", description);
        if (textValdiation.IsFailure)
        {
            return textValdiation.Errors;
        }

        return new FormDescription(description);
    }
    public static implicit operator string(FormDescription description) => description.Value;
}
