using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Domain.AuthActionToken.Enum;

namespace QuickForm.Modules.Users.Application;
public class ForgotPasswordCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IUserRepository userRepository
    ) : ICommandHandler<ForgotPasswordCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resultUser = await GetUser(request.Email);
        if (resultUser.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(resultUser.ResultType, resultUser.Errors);
        }
        var user = resultUser.Value;

        var idAuthActionPasswordRecovery = AuthActionType.RecoveryPassword.GetId();
        var idAuthAction = new AuthActionId(idAuthActionPasswordRecovery);
        user.AddAction(idAuthAction, _dateTimeProvider.UtcNow);

        userRepository.Update(user);

        var result = await ConfirmTransaction(cancellationToken);
        if (result.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(result.ResultType,result.Errors);
        }

        return ResultResponse.Success("Password recovery email sent successfully.");
    }
    private async Task<ResultT<UserDomain>> GetUser(string email)
    {
        var user = await userRepository.GetByEmailWithActiveAuthActionsAsync(email, _dateTimeProvider.UtcNow);
        if (user is null)
        {
            var error = ResultError.InvalidInput("Email", $"User with email '{email}' not found");
            return ResultT<UserDomain>.Failure(ResultType.NotFound, error);
        }
        return user;
    }

    private async Task<Result> ConfirmTransaction(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


}
