using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Application;


internal sealed class RegisterCountryCommandHandler(
        IUnitOfWork _unitOfWork
    ) : ICommandHandler<RegisterCountryCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(RegisterCountryCommand request, CancellationToken cancellationToken)
    {
        var countryCreated = CountryDomain.Create(request.Name, request.Description);

        if (countryCreated.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, countryCreated.Errors);
        }
        _unitOfWork.Repository<CountryDomain, MasterId>().AddEntity(countryCreated.Value);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }
        var countryId = countryCreated.Value.Id; 

        return ResultTResponse<Guid>.Success(countryId.Value, $"Country created successfully id '{countryId}'.");
    }
}
