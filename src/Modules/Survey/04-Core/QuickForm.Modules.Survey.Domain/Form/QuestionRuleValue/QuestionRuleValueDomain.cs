using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionRuleValueDomain : BaseDomainEntity<QuestionRuleValueId>
{
    public QuestionId IdQuestion { get; private set; }
    public QuestionTypeRuleId IdQuestionTypeRule { get; private set; }
    public string? Value { get; private set; }
    public string ? Message { get; set; }

    #region One To Many
    public QuestionDomain Question { get; private set; }
    public QuestionTypeRuleDomain QuestionTypeRule { get; private set; }
    #endregion

    private QuestionRuleValueDomain() { }
    private QuestionRuleValueDomain(
            QuestionRuleValueId id,
            QuestionId idQuestion,
            QuestionTypeRuleId idQuestionTypeRule,
            string? value,
            string message
            
        ) : base(id)
    {
        IdQuestion = idQuestion;
        IdQuestionTypeRule = idQuestionTypeRule;
        Value = value;
        Message = message;
    }

    public static ResultT<QuestionRuleValueDomain> Create(
            QuestionId idQuestion,
            QuestionTypeRuleId idQuestionTypeRule,
            string? value,
            string message)
    {
        var id = QuestionRuleValueId.Create();
        var newDomain = new QuestionRuleValueDomain(id, idQuestion, idQuestionTypeRule, value, message);

        return newDomain;
    }

    public void Update(string? value, string message)
    {
        Value = value;
        Message = message;

    }
}
