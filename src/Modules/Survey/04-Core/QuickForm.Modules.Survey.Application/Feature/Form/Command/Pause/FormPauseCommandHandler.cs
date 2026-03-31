using Microsoft.AspNetCore.Http.HttpResults;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class FormPauseCommandHandler(
    IFormRepository formRepository,
    IUnitOfWork _unitOfWork,
    IFormQueries formQueries
    )
    : ICommandHandler<FormPauseCommand, ResultTResponse<FormViewModel>>
{
    public async Task<ResultT<ResultTResponse<FormViewModel>>> Handle(FormPauseCommand request, CancellationToken cancellationToken)
    {
        var form = await formRepository.GetAsync(request.IdForm, cancellationToken);

        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<ResultTResponse<FormViewModel>>.FailureT(ResultType.NotFound, error);
        }

        var resultUpdate = form.Pause();

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultTResponse<FormViewModel>>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return resultTransaction.Errors;
        }

        var formViewModel = await formQueries.GetFormByIdAsync(
            request.IdForm,
            cancellationToken);

        if (formViewModel is null)
        {
            throw new InvalidOperationException($"Form with id '{request.IdForm}' could not be loaded after pause.");
        }


        return ResultTResponse<FormViewModel>.Success(formViewModel, $"Form '{formViewModel.Name}' paused successfully.");
    }
}
