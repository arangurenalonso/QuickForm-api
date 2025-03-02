using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain.Form;

public sealed record FormId(Guid Value) : EntityId(Value)
{
    public static FormId Create() => new FormId(Guid.NewGuid());
}
