using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record PermissionsActionsDescription
{
    public string Value { get; }

    private PermissionsActionsDescription(string value)
    {
        Value = value;
    }

    public PermissionsActionsDescription()
    {
    }

    public static ResultT<PermissionsActionsDescription> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ResultError.EmptyValue("PermissionsActionsDescription", "Permission Action description cannot be null or empty.");
        }

        if (description.Length > 255)
        {
            return ResultError.InvalidFormat("PermissionsActionsDescription", "Permission Action description must be at most 255 characters long.");
        }

        return new PermissionsActionsDescription(description);
    }

    public static implicit operator string(PermissionsActionsDescription description) => description.Value;
}
