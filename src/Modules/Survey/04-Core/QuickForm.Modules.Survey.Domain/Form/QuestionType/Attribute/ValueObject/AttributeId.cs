using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record AttributeId(Guid Value) : EntityId(Value)
{
    public static AttributeId Create() => new AttributeId(Guid.NewGuid());
}
