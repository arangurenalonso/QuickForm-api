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

        builder.OwnsOne(e => e.DefaultValidationMessageTemplate, owned =>
        {
            owned.Property(v => v.ValidationMessage)
                 .HasColumnName("ValidationMessage_Default")
                 .HasMaxLength(1000)
                 .HasDefaultValue("")
                 .IsRequired();

            owned.Property(v => v.PlaceholderKey)
                 .HasColumnName("ValidationMessage_Placeholder")
                 .HasMaxLength(1000)
                 .IsRequired(false);

        });
    }
}
