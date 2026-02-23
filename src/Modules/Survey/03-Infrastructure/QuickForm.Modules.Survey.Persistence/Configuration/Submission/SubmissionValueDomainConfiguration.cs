using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class SubmissionValueDomainConfiguration : EntityMapBase<SubmissionValueDomain, SubmissionValueId>
{
    protected override void Configure(EntityTypeBuilder<SubmissionValueDomain> builder)
    {
        builder.ToTable("SubmissionValue");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<SubmissionValueId, Guid>(
                    idVO => idVO.Value,
                    guid => new SubmissionValueId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(submissionValue => submissionValue.Submission)
            .WithMany(submission => submission.SubmissionValues)
            .HasForeignKey(submissionValue => submissionValue.IdSubmission)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasOne(submissionValue => submissionValue.Question)
            .WithMany(question => question.SubmissionValues)
            .HasForeignKey(submissionValue => submissionValue.IdQuestion)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}

