using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
namespace QuickForm.Modules.Users.Persistence;
public class RoleRepository(
    UsersDbContext _context
    ) : IRoleRepository
{
    public async Task<RoleDomain?> GetByIdAsync(MasterId roleId)
    {
        var role = await _context.Set<RoleDomain>().FirstOrDefaultAsync(role => role.Id == roleId && !role.IsDeleted);
        return role;
    }
}
