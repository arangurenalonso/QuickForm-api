using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class SubmissionDomain : BaseDomainEntity<SubmissionId>
{
    public FormId IdForm { get; private set; }
    public DateTime SubmittedAtUtc { get; private set; }

    public ICollection<SubmissionValueDomain> SubmissionValues { get; private set; } = [];
    public FormDomain Form { get; private set; }

    private SubmissionDomain() { }

    private SubmissionDomain(SubmissionId id, FormId idForm) : base(id)
    {
        IdForm = idForm;
        SubmittedAtUtc = DateTime.UtcNow;
    }

    public static ResultT<SubmissionDomain> Create(FormId formId)
        => new SubmissionDomain(SubmissionId.Create(), formId);

    public Result AddValue(QuestionId questionId, string valueJson)
    {
        if (SubmissionValues.Any(v => v.IdQuestion == questionId && !v.IsDeleted))
        {
            return ResultError.InvalidOperation(
                "DuplicateAnswer",
                $"Question '{questionId.Value}' was answered more than once in the same submission.");
        }

        var created = SubmissionValueDomain.Create(Id, questionId, valueJson);
        if (created.IsFailure)
        {
            return created.Errors;
        }

        SubmissionValues.Add(created.Value);
        return Result.Success();
    }
}
