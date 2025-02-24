namespace QuickForm.Modules.Users.Domain;

public sealed record AuthActionTokenId(Guid Value)
{
    public static AuthActionTokenId Create() => new AuthActionTokenId(Guid.NewGuid());
}
