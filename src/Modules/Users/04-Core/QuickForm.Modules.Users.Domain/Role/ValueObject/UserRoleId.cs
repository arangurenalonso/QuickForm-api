using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record UserRoleId(Guid Value) : EntityId(Value)
{
    public static UserRoleId Create() => new UserRoleId(Guid.NewGuid());
}
