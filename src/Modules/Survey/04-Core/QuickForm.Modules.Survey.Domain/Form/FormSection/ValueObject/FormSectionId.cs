using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormSectionId(Guid Value) : EntityId(Value)
{
    public static FormSectionId Create() => new FormSectionId(Guid.NewGuid());
}

