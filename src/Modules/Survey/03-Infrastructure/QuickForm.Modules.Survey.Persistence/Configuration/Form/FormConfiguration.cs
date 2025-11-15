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


        builder.OwnsOne(e => e.Name, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Name")
                 .HasMaxLength(255)
                 .IsRequired();
        });

        builder.OwnsOne(e => e.Description, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Description")
                 .HasMaxLength(255)
                 .IsRequired();
        });


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


        builder.HasOne(from => from.Status)
            .WithMany(status => status.Forms)
            .HasForeignKey(from => from.IdStatus)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

    }
}
