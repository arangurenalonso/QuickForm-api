using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record DataTypeDescriptionVO
{
    public string Value { get; }

    private DataTypeDescriptionVO(string value)
    {
        Value = value;
    }

    public DataTypeDescriptionVO()
    {
    }

    public static ResultT<DataTypeDescriptionVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("DataTypeDescription", "DataType description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("DataTypeDescription", "DataType description must be at most 255 characters long.");
        }

        return new DataTypeDescriptionVO(description);
    }

    public static implicit operator string(DataTypeDescriptionVO description) => description.Value;
}
