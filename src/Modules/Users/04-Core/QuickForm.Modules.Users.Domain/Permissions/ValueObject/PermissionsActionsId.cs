using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record PermissionsActionsId(Guid Value) : EntityId(Value)
{
    public static PermissionsActionsId Create() => new PermissionsActionsId(Guid.NewGuid());
}
