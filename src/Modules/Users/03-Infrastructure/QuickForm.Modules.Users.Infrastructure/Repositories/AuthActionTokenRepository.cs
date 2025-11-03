using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence.Repositories;

public class AuthActionTokenRepository(
    UsersDbContext _context
    ) : IAuthActionTokenRepository
{


    public async Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdEmailAndTokenAsync(
        MasterId authActionId,
        EmailVO email,
        TokenVO userActionToken)
    {
        var authActionToken = await _context.AuthActionToken
                                                .FirstOrDefaultAsync(authActionToken => 
                                                                            authActionToken.Token == userActionToken &&
                                                                            authActionToken.IdUserAction == authActionId &&
                                                                            authActionToken.User.Email == email &&
                                                                            !authActionToken.IsDeleted
                                                                            );
        return authActionToken;
    }
}
