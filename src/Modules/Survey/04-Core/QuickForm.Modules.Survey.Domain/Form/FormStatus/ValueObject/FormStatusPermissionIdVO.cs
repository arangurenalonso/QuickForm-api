using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormStatusPermissionIdVO(Guid Value) : EntityId(Value)
{
    public static FormStatusPermissionIdVO Create() => new FormStatusPermissionIdVO(Guid.NewGuid());
}
