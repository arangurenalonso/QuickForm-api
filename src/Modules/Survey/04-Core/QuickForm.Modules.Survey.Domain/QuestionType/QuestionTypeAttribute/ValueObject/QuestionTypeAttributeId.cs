using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed record QuestionTypeAttributeId(Guid Value) : EntityId(Value)
{
    public static QuestionTypeAttributeId Create() => new QuestionTypeAttributeId(Guid.NewGuid());
}
