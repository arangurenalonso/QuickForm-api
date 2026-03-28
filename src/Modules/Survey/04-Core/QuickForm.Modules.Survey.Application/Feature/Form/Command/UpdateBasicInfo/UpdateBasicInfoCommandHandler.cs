using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;


internal sealed class UpdateBasicInfoCommandHandler(IFormRepository formRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<UpdateBasicInfoCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(UpdateBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var form =await formRepository.GetAsync(request.Id, cancellationToken);
        
        if(form == null)
        {
            var error = ResultError.NullValue("FormId",  $"Form with id '{request.Id}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound,error);
        }

        var resultUpdate = form.UpdateBasicInfo(request.Name, request.Description);

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return ResultResponse.Success($"Form {form.Name} Updated successfully.");
    }
}
