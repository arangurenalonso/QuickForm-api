
using System.Globalization;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public record ActionDescriptionVO
{

    public string Value { get; }

    private ActionDescriptionVO(string value)
    {
        Value = value;
    }

    public static ResultT<ActionDescriptionVO> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ResultError.EmptyValue("ActionName", "ActionName cannot be null or empty.");
        }
        var textValdiation = new TextValidationBuilder()
                                    .AddAlphabeticCharacters()
                                    .AddWhitespace()
                                    .Build().ValidateInvalidCharacter("ActionName", name);
        if (textValdiation.IsFailure)
        {
            return textValdiation.Errors;
        }
        return new ActionDescriptionVO(name.ToUpper(CultureInfo.InvariantCulture));

    }
    public static implicit operator string(ActionDescriptionVO name) => name.Value;
}

