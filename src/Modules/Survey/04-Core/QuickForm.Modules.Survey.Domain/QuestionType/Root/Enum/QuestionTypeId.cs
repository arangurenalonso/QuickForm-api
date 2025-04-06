using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionTypeId(Guid Value) : EntityId(Value)
{
    public static QuestionTypeId Create() => new QuestionTypeId(Guid.NewGuid());
}
