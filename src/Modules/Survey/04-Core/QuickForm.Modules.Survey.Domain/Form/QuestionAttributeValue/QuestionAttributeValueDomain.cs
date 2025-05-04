using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionAttributeValueDomain : BaseDomainEntity<QuestionAttributeValueId>
{

    public QuestionId IdQuestion { get; private set; }
    public QuestionTypeAttributeId IdQuestionTypeAttribute { get; private set; }
    public string? Value { get; private set; } 

    #region One To Many
    public QuestionDomain Question { get; private set; }
    public QuestionTypeAttributeDomain QuestionTypeAttribute { get; private set; }
    #endregion

    private QuestionAttributeValueDomain() { }
    private QuestionAttributeValueDomain(
            QuestionAttributeValueId id,
            QuestionId idQuestion,
            QuestionTypeAttributeId idQuestionTypeAttribute, 
            string? value
        ) : base(id)
    {
        IdQuestion = idQuestion;
        IdQuestionTypeAttribute = idQuestionTypeAttribute;
        Value = value;
    }

    public static ResultT<QuestionAttributeValueDomain> Create(
            QuestionId idQuestion,
            QuestionTypeAttributeId idQuestionTypeAttribute,
            string? value)
    {
        var id = QuestionAttributeValueId.Create();
        var newDomain = new QuestionAttributeValueDomain(id, idQuestion, idQuestionTypeAttribute, value);

        return newDomain;
    }

    public void Update(string? value)
    {
        Value = value;
    }
}
