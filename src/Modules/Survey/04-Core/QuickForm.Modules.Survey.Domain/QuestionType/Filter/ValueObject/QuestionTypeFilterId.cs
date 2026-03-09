using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionTypeFilterId(Guid Value) : EntityId(Value)
{
    public static QuestionTypeFilterId Create() => new QuestionTypeFilterId(Guid.NewGuid());
}
