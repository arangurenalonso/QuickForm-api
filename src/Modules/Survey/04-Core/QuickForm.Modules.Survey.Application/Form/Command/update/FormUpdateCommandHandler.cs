using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormUpdateCommandHandler(IFormRepository formRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<FormUpdateCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormUpdateCommand request, CancellationToken cancellationToken)
    {
        var form =await formRepository.GetAsync(new FormId(request.Id), cancellationToken);
        
        if(form == null)
        {
            var error = ResultError.NullValue("FormId",  $"Form with id '{request.Id}' not found.");
            return ResultT<ResultResponse>.Failure(ResultType.NotFound,error);
        }

        var resultUpdate = form.Update(request.Name, request.Description);

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(ResultType.DomainValidation, resultUpdate.Errors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultResponse.Success($"Form id '{request.Id}' Updated successfully.");
    }
}
