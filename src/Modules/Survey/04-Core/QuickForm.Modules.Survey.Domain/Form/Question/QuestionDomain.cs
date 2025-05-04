using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionDomain : BaseDomainEntity<QuestionId>
{

    public FormId IdForm { get; private set; }
    public QuestionTypeId IdQuestionType { get; private set; }
    public int Order { get; private set; }
    #region One To Many
    public FormDomain Form { get; private set; }
    public QuestionTypeDomain QuestionType { get; private set; }
    #endregion
    #region Many to One
    public ICollection<QuestionAttributeValueDomain> QuestionAttributeValue { get; private set; } = [];
    #endregion
    private QuestionDomain() { }
    private QuestionDomain(QuestionId id, FormId idForm, QuestionTypeId idQuestionType,int order) : base(id)
    {
        IdForm = idForm;
        IdQuestionType = idQuestionType;
        Order = order;
    }

    public static ResultT<QuestionDomain> Create(QuestionId id, FormId idForm, QuestionTypeId idQuestionType, int order)
    {
        var newDomain = new QuestionDomain(id, idForm, idQuestionType, order);

        return newDomain;
    }
    public Result Update(int order)
    {
        Order = order;

        return Result.Success();
    }
}
