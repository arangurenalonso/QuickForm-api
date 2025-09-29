using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
namespace QuickForm.Modules.Users.Persistence;
public class UserRepository(
    UsersDbContext _context
    ) : IUserRepository
{


    public async Task<UserDomain?> GetByEmailAsync(EmailVO email)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(usuario => usuario.Email == email && !usuario.IsDeleted);
        return usuario;
    }
    public async Task<UserDomain?> GetByIdAsync(UserId userId)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(usuario => usuario.Id == userId && !usuario.IsDeleted);
        return usuario;
    }

    public async Task<bool> IsEmailExistsAsync(EmailVO email, UserId? userId = null)
    {
        return await _context.Users
                                .AsNoTracking()
                                .AnyAsync(usuario =>
                                (userId == null || usuario.Id != userId) &&
                                usuario.Email == email &&
                                !usuario.IsDeleted);
    }

    public async Task<UserDomain?> GetByEmailWithActiveAuthActionsAsync(EmailVO email, DateTime currentDatetime)
    {
        var currentDateTimeVO = ExpirationDate.Restore(currentDatetime);
        var usuario = await _context.Users
                                    .Include(usuario => usuario.AuthActionTokens.Where(x =>
                                                                                            !x.IsDeleted &&
                                                                                            !x.Used &&
                                                                                            x.ExpiresAt > currentDateTimeVO
                                                                                            )
                                                )
                                    .ThenInclude(userActionToken => userActionToken.Action)
                                    .FirstOrDefaultAsync(usuario => usuario.Email == email && !usuario.IsDeleted);
        return usuario;
    }
    public async Task<UserDomain?> GetByIdWithActiveAuthActionsAsync(UserId userId, DateTime currentDatetime)
    {
        try
        {
            var currentDateTimeVO = ExpirationDate.Restore(currentDatetime);
            var usuario = await _context.Users
                                        .Include(usuario => usuario.AuthActionTokens.Where(x=>
                                                                                                !x.IsDeleted &&
                                                                                                !x.Used&&
                                                                                                x.ExpiresAt> currentDateTimeVO
                                                                                                )
                                                    )
                                        .ThenInclude(userActionToken => userActionToken.Action)
                                        .Where(usuario => usuario.Id == userId && !usuario.IsDeleted)
                                        .AsSplitQuery()
                                        .FirstOrDefaultAsync();
            return usuario;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);   

            throw;
        }
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
