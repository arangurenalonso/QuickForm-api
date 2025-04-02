using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Persistence;
internal sealed class CustomerConfiguration : EntityMapBase<Customer, CustomerId>
{
    protected override void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<CustomerId, Guid>(
                    userId => userId.Value,
                    guid => new CustomerId(guid)
                ))
            .IsRequired();

        builder.Property(c => c.FirstName).HasMaxLength(200);

        builder.Property(c => c.LastName).HasMaxLength(200);

        builder.Property(c => c.Email).HasMaxLength(300);
    }
}
