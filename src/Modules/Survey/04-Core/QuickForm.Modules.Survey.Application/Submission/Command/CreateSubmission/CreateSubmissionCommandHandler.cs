using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class CreateSubmissionCommandHandler(
        IFormRepository formRepository, 
        IUnitOfWork _unitOfWork
    )
    : ICommandHandler<CreateSubmissionCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var form = await formRepository.GetAsync(request.IdForm, cancellationToken);

        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }

        var resultUpdate = form.Close();

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return ResultResponse.Success($"Form id '{request.IdForm}' closed successfully.");
    }
}
