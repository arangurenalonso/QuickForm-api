using System.Text.Json;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;

namespace QuickForm.Modules.Survey.Domain;

public sealed class SubmissionDomain : BaseDomainEntity<SubmissionId>
{
    public FormId IdForm { get; private set; }
    public DateTime SubmittedAtUtc { get; private set; }

    public ICollection<SubmissionValueDomain> SubmissionValues { get; private set; } = [];
    public FormDomain Form { get; private set; } = default!;

    private SubmissionDomain() { }

    private SubmissionDomain(
        SubmissionId id,
        FormId idForm,
        DateTime submittedAtUtc
    ) : base(id)
    {
        IdForm = idForm;
        SubmittedAtUtc = submittedAtUtc;
    }

    public static ResultT<SubmissionDomain> Create(FormId formId, DateTime submittedAtUtc)
    {
        if (formId == default)
        {
            return ResultError.InvalidInput("FormId", "FormId is required.");
        }

        if (submittedAtUtc.Kind != DateTimeKind.Utc)
        {
            submittedAtUtc = DateTime.SpecifyKind(submittedAtUtc, DateTimeKind.Utc);
        }

        return new SubmissionDomain(SubmissionId.Create(), formId, submittedAtUtc);
    }

    public static ResultT<SubmissionDomain> Create(
        FormId formId,
        DateTime submittedAtUtc,
        IReadOnlyList<QuestionForSubmission> questions,
        IReadOnlyDictionary<string, JsonElement> request
    )
    {
        var submissionResult = Create(formId, submittedAtUtc);
        if (submissionResult.IsFailure)
        {
            return ResultT<SubmissionDomain>.FailureT(submissionResult.Errors);
        }

        var addResult = submissionResult.Value.AddSubmissionValue(questions, request);
        if (addResult.IsFailure)
        {
            return ResultT<SubmissionDomain>.FailureT(addResult.Errors);
        }

        return submissionResult;
    }

    public Result AddSubmissionValue(
        IReadOnlyList<QuestionForSubmission> questions,
        IReadOnlyDictionary<string, JsonElement> request
    )
    {
        var normalizedRequestResult = SurveyDomainMethod.NormalizeRequest(request);
        if (normalizedRequestResult.IsFailure)
        {
            return Result.Failure(ResultType.DomainValidation, normalizedRequestResult.Errors);
        }

        var normalizedRequest = normalizedRequestResult.Value;

        var questionsByNameResult = BuildQuestionsByName(questions);
        if (questionsByNameResult.IsFailure)
        {
            return Result.Failure(ResultType.DomainValidation, questionsByNameResult.Errors);
        }

        var questionsByName = questionsByNameResult.Value;

        var invalidKeys = normalizedRequest.Keys
            .Where(k => !questionsByName.ContainsKey(k))
            .ToList();

        if (invalidKeys.Count > 0)
        {
            var error = ResultError.InvalidInput(
                "Request",
                $"The question is not included in the form: {string.Join(", ", invalidKeys)}."
            );
            return Result.Failure(ResultType.DomainValidation, error);
        }

        foreach (var kv in questionsByName)
        {
            var name = kv.Key;
            var question = kv.Value;

            var requiredFlagResult = TryGetRequiredFlag(question);
            if (requiredFlagResult.IsFailure)
            {
                return Result.Failure(ResultType.DomainValidation, requiredFlagResult.Errors);
            }

            var isRequired = requiredFlagResult.Value;
            if (!isRequired)
            {
                continue;
            }

            if (!normalizedRequest.TryGetValue(name, out var val) || CommonJsonElementMethods.IsEmpty(val))
            {
                var error = ResultError.InvalidInput(name, $"The question '{name}' is required.");
                return Result.Failure(ResultType.DomainValidation, error);
            }
        }

        foreach (var (name, value) in normalizedRequest)
        {
            var question = questionsByName[name];

            var requiredFlagResult = TryGetRequiredFlag(question);
            if (requiredFlagResult.IsFailure)
            {
                return Result.Failure(ResultType.DomainValidation, requiredFlagResult.Errors);
            }

            if (!requiredFlagResult.Value && CommonJsonElementMethods.IsEmpty(value))
            {
                continue;
            }

            var submissionValueResult = SubmissionValueDomain.Create(
                Id,
                question,
                name,
                value
            );
            
            if (submissionValueResult.IsFailure)
            {
                return submissionValueResult.Errors;
            }

            SubmissionValues.Add(submissionValueResult.Value);
        }

        return Result.Success();
    }


    private static ResultT<Dictionary<string, QuestionForSubmission>> BuildQuestionsByName(IReadOnlyList<QuestionForSubmission> questions)
    {
        var nameAttributeId = AttributeType.Name.GetId();
        var map = new Dictionary<string, QuestionForSubmission>(StringComparer.OrdinalIgnoreCase);

        foreach (var q in questions)
        {
            var nameAttr = q.Attributes.FirstOrDefault(a => a.AttributeId == nameAttributeId);

            if (string.IsNullOrWhiteSpace(nameAttr.Value))
            {
                var err = ResultError.InvalidInput(
                    "FormDefinition",
                    $"Question '{q.QuestionId.Value}' is missing a valid Name attribute."
                );

                return ResultT<Dictionary<string, QuestionForSubmission>>.FailureT(ResultType.DomainValidation, err);
            }

            var name = nameAttr.Value.Trim();

            if (!map.TryAdd(name, q))
            {
                var err = ResultError.InvalidInput(
                    "FormDefinition",
                    $"Duplicate Name attribute '{name}' found in this form."
                );

                return ResultT<Dictionary<string, QuestionForSubmission>>.FailureT(ResultType.DomainValidation, err);
            }
        }

        return map;
    }

    private static ResultT<bool> TryGetRequiredFlag(QuestionForSubmission question)
    {
        var requiredRule = question.Rules.FirstOrDefault(r => r.RuleId == RuleType.Required.GetId());
        if (requiredRule.Value is null)
        {
            return false; // no rule => not required
        }

        if (!bool.TryParse(requiredRule.Value, out var flag))
        {
            var err = ResultError.InvalidInput(
                "FormDefinition",
                $"Question '{question.QuestionId.Value}' has Required rule with invalid boolean value '{requiredRule.Value}'."
            );
            return ResultT<bool>.FailureT(ResultType.DomainValidation, err);
        }

        return flag;
    }

}
