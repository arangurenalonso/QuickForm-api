using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;


internal sealed class FormRegisterCommandHandler(
        IUnitOfWork _unitOfWork,
        ICurrentUserService currentUserService,
        ICustomerRepository customerRepository
    ) : ICommandHandler<FormRegisterCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormRegisterCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, userIdResult.Errors);
        }
        var customer = await customerRepository.GetAsync(userIdResult.Value, cancellationToken);
        if (customer is null)
        {
            var error = ResultError.InvalidInput("Customer", $"Customer with id '{userIdResult.Value}' not found");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);

        }
        var formCreated = FormDomain.Create(request.Name,request.Description, customer);

        if (formCreated.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, formCreated.Errors);
        }
        _unitOfWork.Repository<FormDomain,FormId>().AddEntity(formCreated.Value);


        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }
        var formId = formCreated.Value.Id; 

        return ResultTResponse<Guid>.Success(formId.Value,$"Form created successfully id '{formId}'.");
    }
}
