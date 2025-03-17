using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record RoleId(Guid Value) : EntityId(Value)
{
    public static RoleId Create() => new RoleId(Guid.NewGuid());
}
