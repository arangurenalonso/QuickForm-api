using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence.Repositories;

public class AuthActionTokenRepository(
    UsersDbContext _context
    ) : IAuthActionTokenRepository
{


    public async Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdAndTokenAsync(
        AuthActionId authActionId,
        string userActionToken)
    {
        var authActionToken = await _context.AuthActionToken.FirstOrDefaultAsync(authActionToken => 
                                                                            authActionToken.Token.Value == userActionToken &&
                                                                            authActionToken.IdUserAction == authActionId &&
                                                                            authActionToken.IsActive);
        return authActionToken;
    }
    public void Insert(AuthActionTokenDomain authActionTokenDomain)
    {
        _context.AuthActionToken.Add(authActionTokenDomain);
    }
    public void Update(AuthActionTokenDomain authActionTokenDomain)
    {
        _context.AuthActionToken.Update(authActionTokenDomain);
    }
}
