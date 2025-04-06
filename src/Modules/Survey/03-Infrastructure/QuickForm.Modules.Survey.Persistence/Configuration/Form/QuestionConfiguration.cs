using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class QuestionConfiguration : EntityMapBase<QuestionDomain, QuestionId>
{
    protected override void Configure(EntityTypeBuilder<QuestionDomain> builder)
    {
        builder.ToTable("Question");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<QuestionId, Guid>(
                    idVo => idVo.Value,
                    guid => new QuestionId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(question => question.Form)
            .WithMany(form => form.Questions)
            .HasForeignKey(question => question.IdForm)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(question => question.QuestionType)
            .WithMany(questionType => questionType.Questions)
            .HasForeignKey(question => question.IdQuestionType)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
