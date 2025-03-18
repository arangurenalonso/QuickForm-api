using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record ResourcesId(Guid Value) : EntityId(Value)
{
    public static ResourcesId Create() => new ResourcesId(Guid.NewGuid());
}
