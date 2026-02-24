using System.Globalization;
using System.Text.Json;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed class SubmissionValueDomain : BaseDomainEntity<SubmissionValueId>
{
    public SubmissionId IdSubmission { get; private set; }
    public QuestionId IdQuestion { get; private set; }
    public string Value { get; private set; } = default!;

    public SubmissionDomain Submission { get; private set; } = default!;
    public QuestionDomain Question { get; private set; } = default!;

    private SubmissionValueDomain() { }

    private SubmissionValueDomain(
        SubmissionValueId id,
        SubmissionId idSubmission,
        QuestionId idQuestion,
        string value
    ) : base(id)
    {
        IdSubmission = idSubmission;
        IdQuestion = idQuestion;
        Value = value;
    }

    public static ResultT<SubmissionValueDomain> Create(
        SubmissionId idSubmission,
        QuestionForSubmission question,
        string name,
        JsonElement value
    )
    {
        var errors = new List<ResultError>();

        var attrValidation = SurveyDomainMethod.ValidateAttributes(value, question, name);
        if (attrValidation.IsFailure)
        {
            errors.AddRange(attrValidation.Errors.ToList());
        }

        // Rule-based validations
        foreach (var rule in question.Rules)
        {
            var validationResult = SurveyDomainMethod.ValidateRules(value, rule, question.QuestionId, name);
            if (validationResult.IsFailure)
            {
                errors.AddRange(validationResult.Errors.ToList());
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return new SubmissionValueDomain(
            SubmissionValueId.Create(),
            idSubmission,
            question.QuestionId,
            value.GetRawText()
        );
    }
}
