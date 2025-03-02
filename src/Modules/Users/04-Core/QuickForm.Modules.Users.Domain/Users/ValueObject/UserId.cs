
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record UserId(Guid Value) : EntityId(Value)
{
    public static UserId Create() => new UserId(Guid.NewGuid());
}
