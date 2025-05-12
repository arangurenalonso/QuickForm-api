using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetFormStructureQueryHandler(
    IFormRepository _formRepository, 
    IUnitOfWork _unitOfWork)
    : ICommandHandler<GetFormStructureQuery, object>
{
    public async Task<ResultT<object>> Handle(GetFormStructureQuery request, CancellationToken cancellationToken)
    {
        var formId = new FormId(request.IdForm);
        var existForm=await _unitOfWork.Repository<FormDomain,FormId>().ExistEntity(formId);
        if (existForm)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }
        var formStructure = await _formRepository.GetStructureFormAsync(request.IdForm, true, cancellationToken);




    }
}
