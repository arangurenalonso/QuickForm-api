
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public interface IRoleRepository
{
    Task<RoleDomain?> GetByIdAsync(MasterId roleId);
}
