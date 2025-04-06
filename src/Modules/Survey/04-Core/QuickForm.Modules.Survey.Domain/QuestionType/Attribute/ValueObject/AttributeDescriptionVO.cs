using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record AttributeDescriptionVO
{
    public string Value { get; }

    private AttributeDescriptionVO(string value)
    {
        Value = value;
    }

    public AttributeDescriptionVO()
    {
    }

    public static ResultT<AttributeDescriptionVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("AttributeDescription", "Attribute description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("AttributeDescription", "Attribute description must be at most 255 characters long.");
        }

        return new AttributeDescriptionVO(description);
    }

    public static implicit operator string(AttributeDescriptionVO description) => description.Value;
}
