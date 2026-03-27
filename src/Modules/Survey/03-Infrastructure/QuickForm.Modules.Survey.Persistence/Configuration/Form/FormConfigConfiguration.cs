using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormConfigConfiguration : EntityMapBase<FormConfigDomain, FormConfigId>
{
    protected override void Configure(EntityTypeBuilder<FormConfigDomain> builder)
    {
        builder.ToTable("FormConfig");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<FormConfigId, Guid>(
                    formId => formId.Value,
                    guid => new FormConfigId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(x => x.FormRenderId)
         .HasConversion(
             id => id.Value,
             value => new MasterId(value))
         .IsRequired();

        builder.HasOne(x => x.Form)
            .WithOne(x => x.FormConfig)
            .HasForeignKey<FormConfigDomain>(x => x.FormId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(x => x.FormId).IsUnique();

        builder.HasOne(x => x.FormRender)
            .WithMany()
            .HasForeignKey(x => x.FormRenderId)
            .OnDelete(DeleteBehavior.Restrict);




    }
}
