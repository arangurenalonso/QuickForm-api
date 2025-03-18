using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record RolePermissionsId(Guid Value) : EntityId(Value)
{
    public static RolePermissionsId Create() => new RolePermissionsId(Guid.NewGuid());
}
