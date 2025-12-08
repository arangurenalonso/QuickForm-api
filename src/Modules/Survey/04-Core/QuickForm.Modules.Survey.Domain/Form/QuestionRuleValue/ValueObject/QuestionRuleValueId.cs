using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record QuestionRuleValueId(Guid Value) : EntityId(Value)
{
    public static QuestionRuleValueId Create() => new QuestionRuleValueId(Guid.NewGuid());
}


