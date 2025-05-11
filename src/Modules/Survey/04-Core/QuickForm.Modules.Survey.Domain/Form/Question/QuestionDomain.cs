using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionDomain : BaseDomainEntity<QuestionId>
{

    public FormSectionId IdFormSection { get; private set; }
    public QuestionTypeId IdQuestionType { get; private set; }
    public int SortOrder { get; private set; }
    #region One To Many
    public FormSectionDomain FormSection { get; private set; }
    public QuestionTypeDomain QuestionType { get; private set; }
    #endregion
    #region Many to One
    public ICollection<QuestionAttributeValueDomain> QuestionAttributeValue { get; private set; } = [];
    #endregion
    private QuestionDomain() { }
    private QuestionDomain(QuestionId id, FormSectionId idFormSection, QuestionTypeId idQuestionType,int sortOrder) : base(id)
    {
        IdFormSection = idFormSection;
        IdQuestionType = idQuestionType;
        SortOrder = sortOrder;
    }

    public static ResultT<QuestionDomain> Create(QuestionId id, FormSectionId idFormSection, QuestionTypeId idQuestionType, int order)
    {
        var newDomain = new QuestionDomain(id, idFormSection, idQuestionType, order);

        return newDomain;
    }
    public Result Update(int sortOrder,FormSectionId formSectionId)
    {
        SortOrder = sortOrder;
        IdFormSection = formSectionId;

        return Result.Success();
    }
}
