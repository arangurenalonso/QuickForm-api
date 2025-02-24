namespace QuickForm.Modules.Users.Domain;


public sealed record AuthActionId(Guid Value)
{
    public static AuthActionId Create() => new AuthActionId(Guid.NewGuid());
}
