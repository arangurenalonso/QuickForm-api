
namespace QuickForm.Modules.Users.Domain;
public interface IRoleRepository
{
    Task<RoleDomain?> GetByIdAsync(RoleId roleId);
}
