using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed record AuthActionTokenId(Guid Value) : EntityId(Value)
{
    public static AuthActionTokenId Create() => new AuthActionTokenId(Guid.NewGuid());
}
