using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormRegisterCommandHandler(IFormRepository formRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<FormRegisterCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormRegisterCommand request, CancellationToken cancellationToken)
    {
        var formCreated = FormDomain.Create(request.Name,request.Description);

        if (formCreated.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, formCreated.Errors);
        }

        formRepository.Insert(formCreated.Value);


        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }
        var formId = formCreated.Value.Id; // Asegúrate de que `Id` existe en `FormDomain`

        return ResultTResponse<Guid>.Success(formId.Value,$"Form created successfully id '{formId}'.");
    }
}
