using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record RoleDescriptionVO
{
    public string Value { get; }

    private RoleDescriptionVO(string value)
    {
        Value = value;
    }

    public RoleDescriptionVO()
    {
    }

    public static ResultT<RoleDescriptionVO> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("RoleDescription", "Role description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("RoleDescription", "Role description must be at most 255 characters long.");
        }

        return new RoleDescriptionVO(description);
    }

    public static implicit operator string(RoleDescriptionVO description) => description.Value;
}
