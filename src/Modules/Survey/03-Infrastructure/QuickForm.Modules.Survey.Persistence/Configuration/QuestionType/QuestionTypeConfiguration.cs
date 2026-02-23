using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionTypeConfiguration : EntityMapBase<QuestionTypeDomain, QuestionTypeId>
{
    protected override void Configure(EntityTypeBuilder<QuestionTypeDomain> builder)
    {
        builder.ToTable("QuestionType");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionTypeId, Guid>(
                    idVO => idVO.Value,
                    guid => new QuestionTypeId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.KeyName)
                .HasColumnName("KeyName")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    keyNameVO => keyNameVO.Value,
                    keyNameString => QuestionTypeKeyNameVO.Create(keyNameString).Value
                    );

        builder.OwnsOne(e => e.Description, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Description")
                 .HasMaxLength(1000)
                 .IsRequired(false);
        });

        builder.HasIndex(p => p.KeyName).IsUnique();


        builder.HasOne(uat => uat.DataType)
            .WithMany(u => u.QuestionType)
            .HasForeignKey(uat => uat.IdDataType)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
