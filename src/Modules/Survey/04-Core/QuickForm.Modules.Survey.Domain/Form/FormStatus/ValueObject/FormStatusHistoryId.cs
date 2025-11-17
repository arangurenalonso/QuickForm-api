using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormStatusHistoryId(Guid Value) : EntityId(Value)
{
    public static FormStatusHistoryId Create() => new FormStatusHistoryId(Guid.NewGuid());
}

