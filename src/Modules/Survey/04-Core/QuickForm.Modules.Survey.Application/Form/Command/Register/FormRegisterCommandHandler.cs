using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormRegisterCommandHandler(IFormRepository formRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<FormRegisterCommand,Guid>
{
    public async Task<ResultT<Guid>> Handle(FormRegisterCommand request, CancellationToken cancellationToken)
    {
        var formCreated = FormDomain.Create(request.Name,request.Description);

        if (formCreated.IsFailure)
        {
            return ResultT<Guid>.Failure(ResultType.DomainValidation, formCreated.Errors);
        }

        formRepository.Insert(formCreated.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return formCreated.Value.Id.Value;
    }
}
