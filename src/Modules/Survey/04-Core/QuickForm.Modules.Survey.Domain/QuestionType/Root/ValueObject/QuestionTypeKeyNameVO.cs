using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionTypeKeyNameVO
{
    public string Value { get; }

    private QuestionTypeKeyNameVO(string value)
    {
        Value = value;
    }

    public QuestionTypeKeyNameVO()
    {
    }

    public static ResultT<QuestionTypeKeyNameVO> Create(string? keyName)
    {
        if (string.IsNullOrWhiteSpace(keyName))
        {
            return ResultError.EmptyValue("KeyName", "KeyName cannot be null or empty.");
        }

        if (keyName.Length > 255)
        {
            return ResultError.InvalidFormat("KeyName", "KeyName must be at most 255 characters long.");
        }
        if (keyName.Contains(' '))
        {
            return ResultError.InvalidFormat("KeyName", "KeyName must not contain spaces.");
        }
        return new QuestionTypeKeyNameVO(keyName);
    }

    public static implicit operator string(QuestionTypeKeyNameVO keyName) => keyName.Value;
}
