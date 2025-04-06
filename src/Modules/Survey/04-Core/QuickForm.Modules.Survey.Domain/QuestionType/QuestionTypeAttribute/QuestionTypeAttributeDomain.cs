using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeAttributeDomain : BaseDomainEntity<QuestionTypeAttributeId>
{
    public QuestionTypeId IdQuestionType { get; private set; }
    public AttributeId IdAttribute { get; private set; }
    public bool IsRequired { get; private set; } 


    #region One to Many
    public QuestionTypeDomain QuestionType { get; private set; }
    public AttributeDomain Attribute { get; private set; }
    #endregion


    #region Many to one
    public ICollection<QuestionAttributeValueDomain> QuestionAttributeValue { get; private set; } = [];
    #endregion
    private QuestionTypeAttributeDomain() { }

    private QuestionTypeAttributeDomain(
        QuestionTypeAttributeId id, QuestionTypeId idQuestionType, AttributeId idAttribute,bool isRequired
        ) : base(id)
    {
        IdQuestionType = idQuestionType;
        IdAttribute = idAttribute;
        IsRequired = isRequired;
    }

    public static ResultT<QuestionTypeAttributeDomain> Create(
        QuestionTypeAttributeId id,
        QuestionTypeId idQuestionType,
        AttributeId idAttribute,
        bool isRequired)
    {
        return new QuestionTypeAttributeDomain(
            id,
            idQuestionType,
            idAttribute,
            isRequired
            );
    }
    public static ResultT<QuestionTypeAttributeDomain> Create(
        QuestionTypeId idQuestionType,
        AttributeId idAttribute,
        bool isRequired)
    {
        return new QuestionTypeAttributeDomain(
            QuestionTypeAttributeId.Create(),
            idQuestionType,
            idAttribute,
            isRequired
            );
    }
    public Result Update(
            QuestionTypeId idQuestionType,
            AttributeId idAttribute,
            bool isRequired
       )
    {
        IsRequired = isRequired;
        IdQuestionType = idQuestionType;
        IdAttribute = idAttribute;
        return Result.Success();
    }

}
