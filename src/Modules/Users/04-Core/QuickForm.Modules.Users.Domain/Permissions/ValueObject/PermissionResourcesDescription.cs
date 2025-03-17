using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record PermissionResourcesDescription
{
    public string Value { get; }

    private PermissionResourcesDescription(string value)
    {
        Value = value;
    }

    public PermissionResourcesDescription()
    {
    }

    public static ResultT<PermissionResourcesDescription> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("PermissionResourcesDescription", "Permission Resource description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("PermissionResourcesDescription", "Permission Resource description must be at most 255 characters long.");
        }

        return new PermissionResourcesDescription(description);
    }

    public static implicit operator string(PermissionResourcesDescription description) => description.Value;
}
