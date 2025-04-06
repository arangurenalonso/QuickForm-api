using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormId(Guid Value) : EntityId(Value)
{
    public static FormId Create() => new FormId(Guid.NewGuid());
}
