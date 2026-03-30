using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record LeadId(Guid Value) : EntityId(Value)
{
    public static LeadId Create() => new LeadId(Guid.NewGuid());
}
