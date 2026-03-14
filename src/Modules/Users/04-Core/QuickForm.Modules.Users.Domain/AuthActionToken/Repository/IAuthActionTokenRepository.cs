using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenRepository
{
    Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdEmailAndTokenHashAsync(
        MasterId authActionId,
        EmailVO email,
        string tokenHash);
}
