using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class ChangePasswordCommandHandler(
        IUnitOfWork _unitOfWork,
        IUserRepository _userRepository,
        IPasswordHashingService _passwordHashingService,
        ICurrentUserService _currentUserService

    ) : ICommandHandler<ChangePasswordCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userConnected = _currentUserService.UserId;
        if (userConnected.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(userConnected.ResultType,userConnected.Errors);
        }

        var resultUser = await ValidateInputData(userConnected.Value);
        if (resultUser.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(resultUser.ResultType,resultUser.Errors);
        }
        var user = resultUser.Value;

        var resultPassword = ValidatePassword(user, request.CurrentPassword);
        if (resultPassword.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(resultPassword.ResultType,resultPassword.Errors);
        }
        var transactionResut = UpdatePassword(user, request.NewPassword);
        if (transactionResut.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(transactionResut.ResultType,transactionResut.Errors);
        }

        var confirmTransactionResult =await _unitOfWork.SaveChangesWithResultAsync(nameof(ChangePasswordCommandHandler), cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(transactionResut.ResultType, transactionResut.Errors);
        }
        return ResultResponse.Success("Password changed successfully.");
    }
    private async Task<ResultT<UserDomain>> ValidateInputData(Guid userId)
    {
        var userIdInstance = new UserId(userId);
        var user = await _userRepository.GetByIdAsync(userIdInstance);
        if (user is null)
        {
            var error = ResultError.InvalidInput("UserId", $"User with id '{userId}' not found");
            return ResultT<UserDomain>.Failure(ResultType.NotFound, error);
        }
        return user;
    }

    private Result ValidatePassword(UserDomain user, string password)
    {
        var result = _passwordHashingService.VerifyPassword(password, user.PasswordHash.Value);

        if (!result)
        {
            var error = ResultError.InvalidInput("Password", $"Invalid Password");
            return Result.Failure(ResultType.DomainValidation, error);
        }
        return Result.Success();

    }
    public Result UpdatePassword(UserDomain user, string newPassword)
    {
        var resultChange = user.ChangePassword(newPassword, _passwordHashingService);
        if (resultChange.IsFailure)
        {
            return Result.Failure(ResultType.DomainValidation, resultChange.Errors);
        }
        _userRepository.Update(user);

        return Result.Success();
    }

}
