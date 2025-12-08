using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionTypeRuleId(Guid Value) : EntityId(Value)
{
    public static QuestionTypeRuleId Create() => new QuestionTypeRuleId(Guid.NewGuid());
}
