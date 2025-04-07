using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormConfiguration : EntityMapBase<FormDomain, FormId>
{
    protected override void Configure(EntityTypeBuilder<FormDomain> builder)
    {
        builder.ToTable("Forms");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<FormId, Guid>(
                    formId => formId.Value,
                    guid => new FormId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Name)
                .HasColumnName("Name")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    nameVO => nameVO.Value,
                    nameString => FormNameVO.Create(nameString).Value
                    );

        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired()
                .HasConversion(
                    formDescriptionVO => formDescriptionVO.Value,
                    formDescription => FormDescription.Create(formDescription).Value
                    );


        builder.Property(p => p.IsPublished)
            .HasColumnName("IsPublished")
            .IsRequired();

        builder.Property(p => p.IsClosed)
            .HasColumnName("IsClosed")
            .IsRequired();


        builder.Property(p => p.DateEnd)
                .HasColumnName("DateEnd")
                .IsRequired(false)
                .HasConversion(
                    new ValueConverter<DateEnd?, DateTime?>(
                        dateEnd => dateEnd == null ? null : dateEnd.Value,
                        date => date.HasValue ? DateEnd.FromDatabase(date) : DateEnd.FromDatabase(null)
                    ));


        builder.HasOne(from => from.Customer)
            .WithMany(customer=>customer.Forms)
            .HasForeignKey(from => from.IdCustomer)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
