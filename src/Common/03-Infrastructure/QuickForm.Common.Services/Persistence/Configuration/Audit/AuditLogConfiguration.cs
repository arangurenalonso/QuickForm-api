using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever(); 

        builder.Property(x => x.IdEntity)
               .IsRequired();

        builder.Property(x => x.TransactionId)
               .IsRequired();

        builder.Property(x => x.TableName)
               .HasMaxLength(128);

        builder.Property(x => x.Action)
               .HasConversion<string?>()
               .HasMaxLength(32);

        builder.Property(x => x.ActionName)
               .HasMaxLength(64);

        builder.Property(x => x.CreatedDate); 

        builder.Property(x => x.UserTransaction)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.ClassOrigin)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.OriginalValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.CurrentValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ChangesValue).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.TableName, x.IdEntity })
               .HasDatabaseName("IX_AuditLog_Table_Entity_Date");

        builder.HasIndex(x => x.TransactionId)
               .HasDatabaseName("IX_AuditLog_TransactionId");

        builder.HasIndex(x => x.CreatedDate)
               .HasDatabaseName("IX_AuditLog_CreatedDate");

        builder.HasIndex(x => x.Action)
               .HasDatabaseName("IX_AuditLog_Action");
    }
}
