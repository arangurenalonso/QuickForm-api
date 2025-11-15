using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure.Persistence;

public abstract class StatusEntityMapBase<TEntity>
    : MasterEntityMapBase<TEntity>
    where TEntity : BaseStatusEntity
{
    protected override void ConfigureMaster(EntityTypeBuilder<TEntity> builder)
    {
        base.ConfigureMaster(builder);

        builder.OwnsOne(e => e.Color, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Color")
                 .HasMaxLength(50)
                 .IsRequired();
        });

        builder.OwnsOne(e => e.Icon, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Icon")
                 .HasMaxLength(100)
                 .IsRequired();
        });

        ConfigureStatus(builder);
    }
    protected virtual void ConfigureStatus(EntityTypeBuilder<TEntity> builder) { }
}
