using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public sealed record RefreshTokenId(Guid Value) : EntityId(Value)
{
    public static RefreshTokenId Create() => new RefreshTokenId(Guid.NewGuid());
}
