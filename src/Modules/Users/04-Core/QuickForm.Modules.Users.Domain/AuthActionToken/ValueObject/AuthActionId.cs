using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;


public sealed record AuthActionId(Guid Value) : EntityId(Value)
{
    public static AuthActionId Create() => new AuthActionId(Guid.NewGuid());
}
