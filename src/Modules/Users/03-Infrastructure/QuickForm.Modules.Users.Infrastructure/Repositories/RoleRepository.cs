using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
namespace QuickForm.Modules.Users.Persistence;
public class RoleRepository(
    UsersDbContext _context
    ) : IRoleRepository
{
    public async Task<RoleDomain?> GetByIdAsync(RoleId roleId)
    {
        var role = await _context.Set<RoleDomain>().FirstOrDefaultAsync(role => role.Id == roleId && !role.IsDeleted);
        return role;
    }
}
