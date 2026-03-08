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

        builder.Property(x => x.ValueRaw)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

        builder.Property(x => x.DisplayValue)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ValueDecimal)
                .HasColumnType("decimal(38,10)");

        builder.Property(x => x.ValueInteger)
                .HasColumnType("bigint");

        builder.Property(x => x.ValueDateTime)
                .HasColumnType("datetime2");

        builder.Property(x => x.ValueBoolean)
                .HasColumnType("bit");
        builder.HasIndex(x => new { x.IdQuestion, x.ValueDecimal });
        builder.HasIndex(x => new { x.IdQuestion, x.ValueInteger });
        builder.HasIndex(x => new { x.IdQuestion, x.ValueDateTime });
        builder.HasIndex(x => new { x.IdQuestion, x.ValueBoolean });


    }
}

