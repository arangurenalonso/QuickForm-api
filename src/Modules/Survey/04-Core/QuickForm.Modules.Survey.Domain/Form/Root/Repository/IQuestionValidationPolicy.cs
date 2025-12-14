using QuickForm.Common.Domain;
namespace QuickForm.Modules.Survey.Domain;

public interface IQuestionValidationPolicy
{
    Result Validate(IReadOnlyList<QuestionToValidate> questions, IReadOnlyList<QuestionTypeDomain> types);
}
