using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class RuleConfiguration : MasterEntityMapBase<RuleDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<RuleDomain> builder)
    {
        builder.ToTable("Rule");


        builder.HasOne(uat => uat.DataType)
            .WithMany(u => u.Rules)
            .HasForeignKey(uat => uat.IdDataType)
            .IsRequired();

    }
}
