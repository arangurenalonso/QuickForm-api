using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class LeadConfiguration : EntityMapBase<LeadDomain, LeadId>
{
    protected override void Configure(EntityTypeBuilder<LeadDomain> builder)
    {
        builder.ToTable("Lead");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<LeadId, Guid>(
                    leadId => leadId.Value,
                    guid => new LeadId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);



    }
}
