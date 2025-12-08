using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeRuleDomain : BaseDomainEntity<QuestionTypeRuleId>
{
    public QuestionTypeId IdQuestionType { get; private set; }
    public MasterId IdRule { get; private set; }
    public bool IsRequired { get; private set; }


    #region One to Many
    public QuestionTypeDomain QuestionType { get; private set; }
    public RuleDomain Rule { get; private set; }
    #endregion


    #region Many to one
    public ICollection<QuestionRuleValueDomain> QuestionRuleValue { get; private set; } = [];
    #endregion
    private QuestionTypeRuleDomain() { }

    private QuestionTypeRuleDomain(
        QuestionTypeRuleId id, 
        QuestionTypeId idQuestionType,
        MasterId idRule, 
        bool isRequired
        ) : base(id)
    {
        IdQuestionType = idQuestionType;
        IdRule = idRule;
        IsRequired = isRequired;
    }

    public static ResultT<QuestionTypeRuleDomain> Create(
        QuestionTypeRuleId id,
        QuestionTypeId idQuestionType,
        MasterId idRule,
        bool isRequired)
    {
        return new QuestionTypeRuleDomain(
            id,
            idQuestionType,
            idRule,
            isRequired
            );
    }
    public static ResultT<QuestionTypeRuleDomain> Create(
        QuestionTypeId idQuestionType,
        MasterId idRule,
        bool isRequired)
    {
        return new QuestionTypeRuleDomain(
            QuestionTypeRuleId.Create(),
            idQuestionType,
            idRule,
            isRequired
            );
    }
    public Result Update(
            QuestionTypeId idQuestionType,
            MasterId idRule,
            bool isRequired
       )
    {
        IdQuestionType = idQuestionType;
        IdRule = idRule;
        IsRequired = isRequired;
        return Result.Success();
    }

}

