using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class ApplicationConfiguration : MasterEntityMapBase<ApplicationDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<ApplicationDomain> builder)
    {
        builder.ToTable("Application");

    }
}
