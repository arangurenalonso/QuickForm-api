using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure.Persistence;
public abstract class EntityMapBase<TEntity, TEntityId>
       : IEntityTypeConfiguration<TEntity>
           where TEntity : BaseDomainEntity<TEntityId>
           where TEntityId : EntityId
{
    void IEntityTypeConfiguration<TEntity>.Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(b => b.CreatedBy).HasMaxLength(100).IsRequired();
        builder.Property(b => b.CreatedDate).IsRequired();
        builder.Property(b => b.ModifiedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(b => b.ModifiedDate).IsRequired(false);
        builder.Property(b => b.DeletedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(b => b.DeletedAt).IsRequired(false);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.HasIndex(b => b.IsDeleted)
               .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        Configure(builder);
    }

    protected abstract void Configure(EntityTypeBuilder<TEntity> builder);
}
