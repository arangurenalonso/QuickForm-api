using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain.Customers;

namespace QuickForm.Modules.Survey.Application;

internal sealed class CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<CreateCustomerCommand>
{
    public async Task<Result> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(request.CustomerId, request.Email, request.FirstName, request.LastName);

        customerRepository.Insert(customer);
        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(nameof(CreateCustomerCommandHandler),cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return Result.Failure(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return Result.Success();
    }
}
