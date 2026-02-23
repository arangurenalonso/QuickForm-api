using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class SubmissionConfiguration : EntityMapBase<SubmissionDomain, SubmissionId>
{
    protected override void Configure(EntityTypeBuilder<SubmissionDomain> builder)
    {
        builder.ToTable("Submission");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<SubmissionId, Guid>(
                    idVO => idVO.Value,
                    guid => new SubmissionId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.HasOne(submission => submission.Form)
            .WithMany(form => form.Submissions)
            .HasForeignKey(submission => submission.IdForm)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
