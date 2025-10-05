using QuickForm.Common.Domain;

namespace QuickForm.Common.Domain;
public sealed record DescriptionVO
{
    public string Value { get; }

    private DescriptionVO(string value)
    {
        Value = value;
    }

    private DescriptionVO()
    {
    }

    public static ResultT<DescriptionVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("Description", "Description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("Description", "Description must be at most 255 characters long.");
        }

        return new DescriptionVO(description);
    }

    public static DescriptionVO FromPersistence(string value) => new DescriptionVO(value);
    public static implicit operator string(DescriptionVO description) => description.Value;
}

