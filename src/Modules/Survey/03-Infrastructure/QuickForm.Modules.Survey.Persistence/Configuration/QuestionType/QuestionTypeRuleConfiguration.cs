using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionTypeRuleConfiguration : EntityMapBase<QuestionTypeRuleDomain, QuestionTypeRuleId>
{
    protected override void Configure(EntityTypeBuilder<QuestionTypeRuleDomain> builder)
    {
        builder.ToTable("QuestionTypeRule");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionTypeRuleId, Guid>(
                    idVO => idVO.Value,
                    guid => new QuestionTypeRuleId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(questionTypeRule => questionTypeRule.Rule)
            .WithMany(rule => rule.QuestionTypeRule)
            .HasForeignKey(questionTypeRule => questionTypeRule.IdRule)
            .IsRequired();

        builder.HasOne(questionTypeRule => questionTypeRule.QuestionType)
            .WithMany(questionType => questionType.QuestionTypeRules)
            .HasForeignKey(questionTypeRule => questionTypeRule.IdQuestionType)
            .IsRequired();



    }
}
