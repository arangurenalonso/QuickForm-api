using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
namespace QuickForm.Modules.Users.Persistence;
public class UserRepository(
    UsersDbContext _context
    ) : IUserRepository
{


    public async Task<UserDomain?> GetByEmailAsync(EmailVO email)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(usuario => usuario.Email == email && usuario.IsActive);
        return usuario;
    }
    public async Task<UserDomain?> GetByIdAsync(UserId userId)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(usuario => usuario.Id == userId && usuario.IsActive);
        return usuario;
    }

    public async Task<bool> IsEmailExistsAsync(EmailVO email, Guid? idUsuario = null)
    {
        return await _context.Users
                                .AsNoTracking()
                                .AnyAsync(usuario =>
                                (idUsuario == null || usuario.Id.Value != idUsuario) &&
                                usuario.Email == email &&
                                usuario.IsActive);
    }

    public async Task<UserDomain?> GetByEmailWithActiveAuthActionsAsync(EmailVO email, DateTime currentDatetime)
    {
        var usuario = await _context.Users
                                    .Include(usuario => usuario.AuthActionTokens.Where(x =>
                                                                                    x.IsActive &&
                                                                                    !x.Used &&
                                                                                    x.ExpiresAt.Value > currentDatetime
                                                                                    ))
                                    .ThenInclude(userActionToken => userActionToken.Action)
                                    .FirstOrDefaultAsync(usuario => usuario.Email == email && usuario.IsActive);
        return usuario;
    }
    public async Task<UserDomain?> GetByIdWithActiveAuthActionsAsync(UserId userId, DateTime currentDatetime)
    {
        var usuario = await _context.Users
                                    .Include(usuario => usuario.AuthActionTokens.Where(x =>
                                                                                    x.IsActive &&
                                                                                    !x.Used &&
                                                                                    x.ExpiresAt.Value > currentDatetime
                                                                                    ))
                                    .ThenInclude(userActionToken => userActionToken.Action)
                                    .FirstOrDefaultAsync(usuario => usuario.Id == userId && usuario.IsActive);
        return usuario;
    }

    public void Insert(UserDomain user)
    {
        _context.Users.Add(user);
    }
    public void Update(UserDomain user)
    {
        _context.Users.Update(user);
    }
}
