using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using QuickForm.Common.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


        //Con OwnsOne: en la query debes comparar por el escalar de la columna, o sea x.KeyName.Value.
        //Con ValueConverter: EF trata el miembro como escalar en la BD.Igual
        //te recomiendo comparar contra escalares para Contains(lista → strings), aunque == con un VO único suele traducir bien.

        ConfigureMaster(builder);
    }

    protected virtual void ConfigureMaster(EntityTypeBuilder<TEntity> builder) { }
}

