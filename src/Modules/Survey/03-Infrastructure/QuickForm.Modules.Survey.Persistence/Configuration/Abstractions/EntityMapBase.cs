using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public abstract class EntityMapBase<TEntity, TEntityId>
       : IEntityTypeConfiguration<TEntity>
           where TEntity : BaseDomainEntity<TEntityId>
           where TEntityId : class
{
    void IEntityTypeConfiguration<TEntity>.Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(b => b.CreatedBy).HasMaxLength(100).IsRequired();
        builder.Property(b => b.CreatedDate).IsRequired();
        builder.Property(b => b.ModifiedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(b => b.ModifiedDate).IsRequired(false);
        builder.Property(b => b.IsActive).HasDefaultValue(true);
        Configure(builder);
    }

    protected abstract void Configure(EntityTypeBuilder<TEntity> builder);
}
