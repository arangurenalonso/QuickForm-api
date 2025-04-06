using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record QuestionId(Guid Value) : EntityId(Value)
{
    public static QuestionId Create() => new QuestionId(Guid.NewGuid());
}
