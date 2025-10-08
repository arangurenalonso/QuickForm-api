using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenRepository
{
    Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdAndTokenAsync(
        MasterId authActionId,
        TokenVO userActionToken);
    void Insert(AuthActionTokenDomain authActionTokenDomain);
    void Update(AuthActionTokenDomain authActionTokenDomain);
}
