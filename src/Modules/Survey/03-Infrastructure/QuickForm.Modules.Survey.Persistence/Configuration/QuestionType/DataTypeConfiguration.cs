using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class DataTypeConfiguration : EntityMapBase<DataTypeDomain, DataTypeId>
{
    protected override void Configure(EntityTypeBuilder<DataTypeDomain> builder)
    {
        builder.ToTable("DataType");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<DataTypeId, Guid>(
                    idVO => idVO.Value,
                    guid => new DataTypeId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => DataTypeDescriptionVO.Create(descriptionString).Value
                    );

    }
}
