using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure.Persistence;
public abstract class MasterEntityMapBase<TEntity>
    : EntityMapBase<TEntity, MasterId>
    where TEntity : BaseMasterEntity
{
    protected override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<MasterId, Guid>(
                    idVO => idVO.Value,
                    guid => new MasterId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.Property(e => e.SortOrder)
               .HasDefaultValue(0);


        builder.OwnsOne(e => e.KeyName, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("KeyName")
                 .HasMaxLength(200)
                 .IsRequired();

            owned.HasIndex(v => v.Value)
                 .HasDatabaseName($"UX_{typeof(TEntity).Name}_KeyName");
        });

        builder.OwnsOne(e => e.Description, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Description")
                 .IsRequired(false);
        });


        //builder.Property(p => p.Description)
        //        .HasColumnName("Description")
        //        .IsRequired(false)
        //        .HasConversion(
        //            new ValueConverter<DescriptionVO?, string?>(
        //                descriptionVO => descriptionVO == null ? null : descriptionVO.Value,
        //                descriptionString => 
        //                        descriptionString!=null 
        //                            ? DescriptionVO.FromPersistence(descriptionString)
        //                            : null
        //            ))


        ConfigureMaster(builder);
    }

    protected virtual void ConfigureMaster(EntityTypeBuilder<TEntity> builder) { }
}

