using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class FormPauseCommandHandler(IFormRepository formRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<FormPauseCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormPauseCommand request, CancellationToken cancellationToken)
    {
        var form = await formRepository.GetAsync(request.Id, cancellationToken);

        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.Id}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }

        var resultUpdate = form.Pause();

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return resultTransaction.Errors;
        }

        return ResultResponse.Success($"Form '{form.Name.Value}' pause successfully.");
    }
}
