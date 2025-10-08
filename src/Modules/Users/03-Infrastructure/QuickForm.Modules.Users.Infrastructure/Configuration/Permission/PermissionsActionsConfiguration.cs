using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class PermissionsActionsConfiguration : MasterEntityMapBase<PermissionsActionsDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<PermissionsActionsDomain> builder)
    {
        builder.ToTable("PermissionsActions");

    }
}
