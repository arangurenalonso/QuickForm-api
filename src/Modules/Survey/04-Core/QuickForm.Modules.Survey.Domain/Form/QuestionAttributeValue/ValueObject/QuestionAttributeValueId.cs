using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed record QuestionAttributeValueId(Guid Value) : EntityId(Value)
{
    public static QuestionAttributeValueId Create() => new QuestionAttributeValueId(Guid.NewGuid());
}
