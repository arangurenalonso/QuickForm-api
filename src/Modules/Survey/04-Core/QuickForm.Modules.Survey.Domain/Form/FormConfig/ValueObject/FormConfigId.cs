using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormConfigId(Guid Value) : EntityId(Value)
{
    public static FormConfigId Create() => new FormConfigId(Guid.NewGuid());
}

