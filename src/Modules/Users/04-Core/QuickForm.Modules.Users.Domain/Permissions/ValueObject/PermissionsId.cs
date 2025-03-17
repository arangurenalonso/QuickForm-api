using System.Security;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record PermissionsId(Guid Value) : EntityId(Value)
{
    public static PermissionsId Create() => new PermissionsId(Guid.NewGuid());
}
