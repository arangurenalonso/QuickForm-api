using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenRepository
{
    Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdEmailAndTokenAsync(
        MasterId authActionId,
        EmailVO email,
        TokenVO userActionToken);
}
