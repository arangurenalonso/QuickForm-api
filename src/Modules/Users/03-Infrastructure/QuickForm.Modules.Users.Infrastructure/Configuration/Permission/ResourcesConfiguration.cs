using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class ResourcesConfiguration : MasterEntityMapBase<ResourcesDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<ResourcesDomain> builder)
    {
        builder.ToTable("Resources");

    }
}
