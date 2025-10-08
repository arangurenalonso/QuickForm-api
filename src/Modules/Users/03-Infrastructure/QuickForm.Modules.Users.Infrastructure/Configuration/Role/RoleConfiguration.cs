using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class RoleConfiguration : MasterEntityMapBase<RoleDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<RoleDomain> builder)
    {
        builder.ToTable("Role");

    }
}
