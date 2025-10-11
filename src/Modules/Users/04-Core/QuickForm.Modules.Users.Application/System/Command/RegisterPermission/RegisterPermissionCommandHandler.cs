using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;
public class RegisterPermissionCommandHandler(
        IUnitOfWork _unitOfWork
    ) : ICommandHandler<RegisterPermissionCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(RegisterPermissionCommand request, CancellationToken cancellationToken)
    {

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }

        return ResultResponse.Success("User created successfully.");
    }

}

