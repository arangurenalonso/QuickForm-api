using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionTypeFilterConfiguration : EntityMapBase<QuestionTypeFilterDomain, QuestionTypeFilterId>
{
    protected override void Configure(EntityTypeBuilder<QuestionTypeFilterDomain> builder)
    {
        builder.ToTable("QuestionTypeFilter");
        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionTypeFilterId, Guid>(
                    idVO => idVO.Value,
                    guid => new QuestionTypeFilterId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(uat => uat.UiControlType)
            .WithMany(u => u.QuestionTypeFilter)
            .HasForeignKey(uat => uat.IdUiControlType)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();


        builder.HasOne(uat => uat.ConditionOperator)
            .WithMany(u => u.QuestionTypeFilter)
            .HasForeignKey(uat => uat.IdConditionalOperator)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();


        builder.HasOne(uat => uat.QuestionType)
            .WithMany(u => u.QuestionTypeFilter)
            .HasForeignKey(uat => uat.IdQuestionType)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}


