using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record PermissionResourcesId(Guid Value) : EntityId(Value)
{
    public static PermissionResourcesId Create() => new PermissionResourcesId(Guid.NewGuid());
}
