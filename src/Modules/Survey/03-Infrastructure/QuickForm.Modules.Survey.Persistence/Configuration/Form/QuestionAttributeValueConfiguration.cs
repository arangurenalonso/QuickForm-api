using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionAttributeValueConfiguration : EntityMapBase<QuestionAttributeValueDomain, QuestionAttributeValueId>
{
    protected override void Configure(EntityTypeBuilder<QuestionAttributeValueDomain> builder)
    {
        builder.ToTable("QuestionAttributeValue");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionAttributeValueId, Guid>(
                    idVo => idVo.Value,
                    guid => new QuestionAttributeValueId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(questionAttributeValueDomain => questionAttributeValueDomain.Question)
            .WithMany(question => question.QuestionAttributeValue)
            .HasForeignKey(questionAttributeValueDomain => questionAttributeValueDomain.IdQuestion)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(questionAttributeValueDomain => questionAttributeValueDomain.QuestionTypeAttribute)
            .WithMany(questionTypeAttribute => questionTypeAttribute.QuestionAttributeValue)
            .HasForeignKey(questionAttributeValueDomain => questionAttributeValueDomain.IdQuestionTypeAttribute)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
