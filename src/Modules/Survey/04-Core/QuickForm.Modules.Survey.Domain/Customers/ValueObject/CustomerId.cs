using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record CustomerId(Guid Value) : EntityId(Value)
{
    public static CustomerId Create() => new CustomerId(Guid.NewGuid());
}
