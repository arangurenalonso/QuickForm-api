using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionTypeAttributeConfiguration : EntityMapBase<QuestionTypeAttributeDomain, QuestionTypeAttributeId>
{
    protected override void Configure(EntityTypeBuilder<QuestionTypeAttributeDomain> builder)
    {
        builder.ToTable("QuestionTypeAttribute");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionTypeAttributeId, Guid>(
                    idVO => idVO.Value,
                    guid => new QuestionTypeAttributeId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(uat => uat.Attribute)
            .WithMany(u => u.QuestionTypeAttributes)
            .HasForeignKey(uat => uat.IdAttribute)
            .IsRequired();

        builder.HasOne(uat => uat.QuestionType)
            .WithMany(u => u.QuestionTypeAttributes)
            .HasForeignKey(uat => uat.IdQuestionType)
            .IsRequired();



    }
}
