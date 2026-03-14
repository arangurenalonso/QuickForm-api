using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence.Repositories;

public class AuthActionTokenRepository(
    UsersDbContext _context
) : IAuthActionTokenRepository
{
    public async Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdEmailAndTokenHashAsync(
        MasterId authActionId,
        EmailVO email,
        string tokenHash)
    {
        var authActionToken = await _context.Set<AuthActionTokenDomain>()
            .FirstOrDefaultAsync(authActionToken =>
                authActionToken.TokenHash == tokenHash &&
                authActionToken.IdUserAction == authActionId &&
                authActionToken.User.Email == email &&
                !authActionToken.IsDeleted);

        return authActionToken;
    }
}
