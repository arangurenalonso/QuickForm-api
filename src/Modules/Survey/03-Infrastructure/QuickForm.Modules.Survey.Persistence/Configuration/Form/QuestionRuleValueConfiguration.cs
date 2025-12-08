using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionRuleValueConfiguration : EntityMapBase<QuestionRuleValueDomain, QuestionRuleValueId>
{
    protected override void Configure(EntityTypeBuilder<QuestionRuleValueDomain> builder)
    {
        builder.ToTable("QuestionRuleValue");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionRuleValueId, Guid>(
                    idVo => idVo.Value,
                    guid => new QuestionRuleValueId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(questionRuleValueDomain => questionRuleValueDomain.Question)
            .WithMany(question => question.QuestionRuleValue)
            .HasForeignKey(questionRuleValueDomain => questionRuleValueDomain.IdQuestion)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(questionRuleValueDomain => questionRuleValueDomain.QuestionTypeRule)
            .WithMany(questionTypeRule => questionTypeRule.QuestionRuleValue)
            .HasForeignKey(questionRuleValueDomain => questionRuleValueDomain.IdQuestionTypeRule)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}

