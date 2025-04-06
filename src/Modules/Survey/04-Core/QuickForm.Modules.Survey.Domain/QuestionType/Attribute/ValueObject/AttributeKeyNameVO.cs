using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record AttributeKeyNameVO
{
    public string Value { get; }

    private AttributeKeyNameVO(string value)
    {
        Value = value;
    }

    public AttributeKeyNameVO()
    {
    }

    public static ResultT<AttributeKeyNameVO> Create(string? keyName)
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
        return new AttributeKeyNameVO(keyName);
    }

    public static implicit operator string(AttributeKeyNameVO keyName) => keyName.Value;
}
