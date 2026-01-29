using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeRuleDomain : BaseDomainEntity<QuestionTypeRuleId>
{
    public QuestionTypeId IdQuestionType { get; private set; }
    public MasterId IdRule { get; private set; }
    public bool IsRequired { get; private set; }
    public string DefaultValidationMessage { get; private set; } = string.Empty;

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
        bool isRequired,
        string defaultValidationMessage
        ) : base(id)
    {
        IdQuestionType = idQuestionType;
        IdRule = idRule;
        IsRequired = isRequired;
        DefaultValidationMessage = defaultValidationMessage;
    }

    public static ResultT<QuestionTypeRuleDomain> Create(
        QuestionTypeRuleId id,
        QuestionTypeId idQuestionType,
        MasterId idRule,
        bool isRequired,
        string defaultValidationMessage)
    {
        return new QuestionTypeRuleDomain(
            id,
            idQuestionType,
            idRule,
            isRequired,
            defaultValidationMessage
            );
    }
    public static ResultT<QuestionTypeRuleDomain> Create(
        QuestionTypeId idQuestionType,
        MasterId idRule,
        bool isRequired,
        string defaultValidationMessage)
    {
        return new QuestionTypeRuleDomain(
            QuestionTypeRuleId.Create(),
            idQuestionType,
            idRule,
            isRequired,
            defaultValidationMessage
            );
    }
    public Result Update(
            QuestionTypeId idQuestionType,
            MasterId idRule,
            bool isRequired,
            string defaultValidationMessage
       )
    {
        IdQuestionType = idQuestionType;
        IdRule = idRule;
        IsRequired = isRequired;
        DefaultValidationMessage = defaultValidationMessage;
        return Result.Success();
    }

}

