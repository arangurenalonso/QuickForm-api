using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record FormStatusPermissionId(Guid Value) : EntityId(Value)
{
    public static FormStatusPermissionId Create() => new FormStatusPermissionId(Guid.NewGuid());
}
