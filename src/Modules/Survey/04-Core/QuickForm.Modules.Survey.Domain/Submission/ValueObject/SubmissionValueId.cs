using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record SubmissionValueId(Guid Value) : EntityId(Value)
{
    public static SubmissionValueId Create() => new SubmissionValueId(Guid.NewGuid());
}

