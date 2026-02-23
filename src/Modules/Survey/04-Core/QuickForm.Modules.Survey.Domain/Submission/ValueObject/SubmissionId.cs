using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record SubmissionId(Guid Value) : EntityId(Value)
{
    public static SubmissionId Create() => new SubmissionId(Guid.NewGuid());
}
