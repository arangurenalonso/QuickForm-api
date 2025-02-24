namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenRepository
{
    Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdAndTokenAsync(
        AuthActionId authActionId,
        string userActionToken);
    void Insert(AuthActionTokenDomain authActionTokenDomain);
    void Update(AuthActionTokenDomain authActionTokenDomain);
}
