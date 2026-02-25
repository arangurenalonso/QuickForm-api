using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class CreateSubmissionCommandHandler(
        IDateTimeProvider _dateTimeProvider,
        IFormRepository formRepository, 
        IFormQueries formQueries,
        IUnitOfWork _unitOfWork
    )
    : ICommandHandler<CreateSubmissionCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var form = await formRepository.GetFormToCheckActionAsync(request.IdForm, cancellationToken);

        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }
        var canSubmitResult = form.EnsureCanPerformAction(FormActionType.AllowSubmission);
        if (canSubmitResult.IsFailure)
        {
            return canSubmitResult.Errors;
        }

        var now = _dateTimeProvider.UtcNow;

        var questionsSubmission = await formQueries.GetQuestionsForSubmissionAsync(request.IdForm, cancellationToken);

        var newSubmission = SubmissionDomain.Create(form.Id, now, questionsSubmission, request.request);
        if (newSubmission.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(newSubmission.Errors);
        }
        _unitOfWork.Repository<SubmissionDomain,SubmissionId>().AddEntity(newSubmission.Value);

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return resultTransaction.Errors;
        }
        return ResultResponse.Success($"Submission was save succesfully.");

    }
}

