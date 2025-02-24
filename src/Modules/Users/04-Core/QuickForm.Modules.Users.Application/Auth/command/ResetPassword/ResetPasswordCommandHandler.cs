using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Domain.AuthActionToken.Enum;

namespace QuickForm.Modules.Users.Application;
public class ResetPasswordCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IPasswordHashingService _passwordHashingService,
    IUserRepository _userRepository,
    IAuthActionTokenRepository _authActionTokenRepository
    ) : ICommandHandler<ResetPasswordCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var authActionTokenResult = await GetUserByAuthActionToken(request.Token);
        if (authActionTokenResult.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(authActionTokenResult.ResultType,authActionTokenResult.Errors);
        }
        var authActionToken = authActionTokenResult.Value;
        var userDomainResult = await ValidateInputData(authActionToken.IdUser);
        if (userDomainResult.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(userDomainResult.ResultType,userDomainResult.Errors);
        }
        var user = userDomainResult.Value;
        user.ChangePassword(request.Password, _passwordHashingService);
        authActionToken.UseToken();

        _userRepository.Update(user);
        _authActionTokenRepository.Update(authActionToken);

        var result = await ConfirmTransaction(cancellationToken);
        if (result.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(result.ResultType,result.Errors);
        }
        return ResultResponse.Success("Your password has been reset successfully.");

    }
    private async Task<ResultT<AuthActionTokenDomain>> GetUserByAuthActionToken(string token)
    {
        var idAuthActionRecoveryPassword = AuthActionType.RecoveryPassword.GetId();
        var idAuthAction = new AuthActionId(idAuthActionRecoveryPassword);

        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdAndTokenAsync(idAuthAction, token);
        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                $"The password recovery token '{token}' could not be found. Please ensure that you have entered a valid token."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.NotFound,error);
        }

        if (authActionToken.Used)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The password recovery token '{token}' has already been used. Please request a new token if needed."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.DomainValidation, error);
        }

        if (authActionToken.ExpiresAt.Value <= _dateTimeProvider.UtcNow)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The password recovery token '{token}' has expired. Please request a new token to proceed with the confirmation."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.DomainValidation,error);
        }

        return authActionToken;
    }

    private async Task<ResultT<UserDomain>> ValidateInputData(UserId userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            var error = ResultError.NullValue("UserId", $"User with id '{userId}' not found.");
            return ResultT<UserDomain>.Failure(ResultType.NotFound,error);
        }
        return user;
    }

    private async Task<Result> ConfirmTransaction(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
