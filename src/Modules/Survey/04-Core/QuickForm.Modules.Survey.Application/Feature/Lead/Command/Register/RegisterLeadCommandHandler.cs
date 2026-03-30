using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;


internal sealed class RegisterLeadCommandHandler(
        IUnitOfWork _unitOfWork
    )
    : ICommandHandler<RegisterLeadCommand, ResultTResponse<Guid>>
{
    public async Task<ResultT<ResultTResponse<Guid>>> Handle(RegisterLeadCommand request, CancellationToken cancellationToken)
    {


        var leadCreated = LeadDomain.Create(request.Name, request.Email,request.PhoneNumber);

        if (leadCreated.IsFailure)
        {
            return leadCreated.Errors;
        }
        _unitOfWork.Repository<LeadDomain, LeadId>().AddEntity(leadCreated.Value);


        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return resultTransaction.Errors;
        }
        var leadId = leadCreated.Value.Id;

        return ResultTResponse<Guid>.Success(leadId.Value, $"Lead created successfully id '{leadId.Value}'.");
    }
}
