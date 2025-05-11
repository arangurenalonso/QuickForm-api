using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormSectionConfiguration : EntityMapBase<FormSectionDomain, FormSectionId>
{
    protected override void Configure(EntityTypeBuilder<FormSectionDomain> builder)
    {
        builder.ToTable("FormSection");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<FormSectionId, Guid>(
                    formId => formId.Value,
                    guid => new FormSectionId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Title)
                .HasColumnName("Title")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    titleVO => titleVO.Value,
                    titleString => FormSectionTitle.Create(titleString).Value
                    );

        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired()
                .HasConversion(
                    formDescriptionVO => formDescriptionVO.Value,
                    formDescription => FormSectionsDescription.Create(formDescription).Value
                    );

    }
}
