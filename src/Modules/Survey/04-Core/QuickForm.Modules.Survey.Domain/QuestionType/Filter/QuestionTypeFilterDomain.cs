using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeFilterDomain : BaseDomainEntity<QuestionTypeFilterId>
{

    public MasterId IdConditionalOperator { get; private set; }
    public MasterId IdUiControlType { get; private set; }
    public QuestionTypeId IdQuestionType { get; private set; }

    #region One To Many
    public QuestionTypeDomain QuestionType { get; private set; }
    public ConditionalOperatorDomain ConditionalOperator { get; private set; }
    public UiControlTypeDomain UiControlType { get; private set; }
    #endregion


    private QuestionTypeFilterDomain() { }

    private QuestionTypeFilterDomain(
        QuestionTypeFilterId id,
        MasterId idConditionalOperator,
        MasterId idUiControlType,
        QuestionTypeId idQuestionType
        ) : base(id)
    {
        IdConditionalOperator = idConditionalOperator;
        IdUiControlType = idUiControlType;
        IdQuestionType = idQuestionType;
    }

    public static ResultT<QuestionTypeFilterDomain> Create(
        QuestionTypeFilterId id,
        MasterId idConditionalOperator,
        MasterId idUiControlType,
        QuestionTypeId idQuestionType
    )
    {
        var domain = new QuestionTypeFilterDomain(id, idConditionalOperator, idUiControlType, idQuestionType);
        return domain;
    }
    public static ResultT<QuestionTypeFilterDomain> Create(
        MasterId idConditionalOperator,
        MasterId idUiControlType,
        QuestionTypeId idQuestionType
    )
        => Create(QuestionTypeFilterId.Create(), idConditionalOperator, idUiControlType, idQuestionType);

    public Result Update(
        MasterId idConditionalOperator,
        MasterId idUiControlType,
        QuestionTypeId idQuestionType
    )
    {
        IdConditionalOperator = idConditionalOperator;
        IdUiControlType = idUiControlType;
        IdQuestionType = idQuestionType;
        return Result.Success();
    }
}
