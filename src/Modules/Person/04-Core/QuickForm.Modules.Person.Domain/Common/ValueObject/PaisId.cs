using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;

public sealed record PaisId(Guid Value) : EntityId(Value)
{
    public static PaisId Create() => new PaisId(Guid.NewGuid());
}
