using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class UiControlTypeConfiguration : MasterEntityMapBase<UiControlTypeDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<UiControlTypeDomain> builder)
    {
        builder.ToTable("UiControlType");


    }
}
