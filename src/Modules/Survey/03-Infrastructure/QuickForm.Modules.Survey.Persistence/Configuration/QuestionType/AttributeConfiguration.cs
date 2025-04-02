using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class AttributeConfiguration : EntityMapBase<AttributeDomain, AttributeId>
{
    protected override void Configure(EntityTypeBuilder<AttributeDomain> builder)
    {
        builder.ToTable("Attribute");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<AttributeId, Guid>(
                    idVO => idVO.Value,
                    guid => new AttributeId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.KeyName)
                .HasColumnName("KeyName")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    keyNameVO => keyNameVO.Value,
                    keyNameString => AttributeKeyNameVO.Create(keyNameString).Value
                    );

        builder.HasIndex(p => p.KeyName).IsUnique();

        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => AttributeDescriptionVO.Create(descriptionString).Value
                    );


        builder.HasOne(uat => uat.DataType)
            .WithMany(u => u.Attributes)
            .HasForeignKey(uat => uat.IdDataType)
            .IsRequired();



    }
}
