
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record PersonId(Guid Value) : EntityId(Value)
{
    public static PersonId Create() => new PersonId(Guid.NewGuid());
}
