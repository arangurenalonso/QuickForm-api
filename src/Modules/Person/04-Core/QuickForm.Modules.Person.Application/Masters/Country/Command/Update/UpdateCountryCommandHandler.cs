using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Application;
internal sealed class UpdateCountryCommandHandler(
        IUnitOfWork _unitOfWork
    )
    : ICommandHandler<UpdateCountryCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var masterId = new MasterId(request.Id);
        var country =await _unitOfWork.Repository<CountryDomain, MasterId>()
                                        .GetById(masterId,false, cancellationToken);

        if (country == null)
        {
            var error = ResultError.NullValue("CountryId", $"Country with id '{request.Id}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }

        var resultUpdate = country.Update(request.Name, request.Description);

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }

        return ResultResponse.Success($"Country successfully updated.");
    }
}
