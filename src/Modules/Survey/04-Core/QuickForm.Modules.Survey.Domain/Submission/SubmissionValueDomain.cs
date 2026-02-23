using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class SubmissionValueDomain : BaseDomainEntity<SubmissionValueId>
{
    public SubmissionId IdSubmission { get; private set; }
    public QuestionId IdQuestion { get; private set; }

    // store raw JSON text (keeps number/bool/string)
    public string ValueJson { get; private set; } = default!;

    private SubmissionValueDomain() { }

    private SubmissionValueDomain(
        SubmissionValueId id,
        SubmissionId idSubmission,
        QuestionId idQuestion,
        string valueJson
    ) : base(id)
    {
        IdSubmission = idSubmission;
        IdQuestion = idQuestion;
        ValueJson = valueJson;
    }

    public static ResultT<SubmissionValueDomain> Create(
        SubmissionId idSubmission,
        QuestionId idQuestion,
        string valueJson
    )
    {
        if (string.IsNullOrWhiteSpace(valueJson))
        {
            // allow "null" too if you want; this is just a guard example
            return ResultError.InvalidInput("Value", "ValueJson cannot be empty.");
        }

        return new SubmissionValueDomain(
            SubmissionValueId.Create(),
            idSubmission,
            idQuestion,
            valueJson
        );
    }
}
