
using QuickForm.Common.Domain;

namespace QuickForm.Common.Domain;

public sealed record MasterId(Guid Value) : EntityId(Value)
{
    public static MasterId Create() => new MasterId(Guid.NewGuid());
}
