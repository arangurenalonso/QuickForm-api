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
                 .HasDatabaseName($"IX_{typeof(TEntity).Name}_KeyName");
        });

        builder.OwnsOne(e => e.Description, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Description")
                 .HasMaxLength(1000)
                 .IsRequired(false);
        });

        //El prefijo UX_ normalmente se usa para índices únicos (Unique Index)
        // El prfijo IX_ se usa para índices no únicos (Index)

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

        //1) Si usas OwnsOne
        //La columna real es "KeyName"(string).Entonces:

        //      var needles = keyNames.Select(k => k.Value).ToList()
        //      var query = _context.Set<TEntity>()
        //                          .Where(x => needles.Contains(x.KeyName.Value) && !x.IsDeleted)

        //Contains debe ser sobre una lista de strings, y del lado de la entidad usar x.KeyName.Value.

        //2) Si usas ValueConverter(recomendado para VO mono - escalar)
        //Aquí EF convierte KeyNameVO ⇄ string al hablar con la BD.
        //Comparación por igualdad con un VO único(ok):
        //        var vo = KeyNameVO.Create("APP_ADMIN").Value
        //        var q = _context.Set<TEntity>().Where(x => x.KeyName == vo)
        //Esto suele traducir correctamente(EF aplica el converter al parámetro).


        ConfigureMaster(builder);
    }

    protected virtual void ConfigureMaster(EntityTypeBuilder<TEntity> builder) { }
}

