using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormStatusHistoryConfiguration : EntityMapBase<FormStatusHistoryDomain, FormStatusHistoryId>
{
    protected override void Configure(EntityTypeBuilder<FormStatusHistoryDomain> builder)
    {
        builder.ToTable("FormStatusHistory");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<FormStatusHistoryId, Guid>(
                    idVO => idVO.Value,
                    guid => new FormStatusHistoryId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.HasOne(history => history.FormStatus)
            .WithMany(status => status.StatusHistory)
            .HasForeignKey(history => history.IdFormStatus)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasOne(history => history.Form)
            .WithMany(form => form.StatusHistory)
            .HasForeignKey(history => history.IdForm)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);


    }
}
