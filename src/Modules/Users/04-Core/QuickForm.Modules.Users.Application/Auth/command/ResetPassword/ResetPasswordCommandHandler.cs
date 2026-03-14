using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class ResetPasswordCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IPasswordHashingService _passwordHashingService,
    IUserRepository _userRepository,
    IAuthActionTokenRepository _authActionTokenRepository,
    IAuthActionTokenHashingService _authActionTokenHashingService
) : ICommandHandler<ResetPasswordCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var userDomainResult = await GetUserByEmail(request.Email);
        if (userDomainResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(userDomainResult.ResultType, userDomainResult.Errors);
        }

        var user = userDomainResult.Value;

        var authActionTokenResult = await GetUserByAuthActionToken(user.Email, request.Token);
        if (authActionTokenResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(authActionTokenResult.ResultType, authActionTokenResult.Errors);
        }

        var authActionToken = authActionTokenResult.Value;

        var changePasswordResult = user.ChangePassword(request.Password, _passwordHashingService);
        if (changePasswordResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(changePasswordResult.ResultType, changePasswordResult.Errors);
        }

        var useTokenResult = authActionToken.UseToken(_dateTimeProvider.UtcNow);
        if (useTokenResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(useTokenResult.ResultType, useTokenResult.Errors);
        }

        _unitOfWork.Repository<UserDomain, UserId>().UpdateEntity(user);
        _unitOfWork.Repository<AuthActionTokenDomain, AuthActionTokenId>().UpdateEntity(authActionToken);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }

        var result = ResultResponse.Success("Password has been reset successfully. You can now log in with your new password.");
        result.WithLink("/auth/login");
        return result;
    }

    private async Task<ResultT<AuthActionTokenDomain>> GetUserByAuthActionToken(EmailVO email, string token)
    {
        var tokenHashResult = _authActionTokenHashingService.Hash(token);
        if (tokenHashResult.IsFailure)
        {
            return ResultT<AuthActionTokenDomain>.FailureT(tokenHashResult.ResultType, tokenHashResult.Errors);
        }

        var idAuthAction = new MasterId(AuthActionType.RecoveryPassword.GetId());

        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdEmailAndTokenHashAsync(
            idAuthAction,
            email,
            tokenHashResult.Value);

        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                "The password recovery token could not be found. Please ensure that you have entered a valid token.");
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.NotFound, error);
        }

        return authActionToken;
    }

    private async Task<ResultT<UserDomain>> GetUserByEmail(string email)
    {
        var emailResult = EmailVO.Create(email);
        if (emailResult.IsFailure)
        {
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, emailResult.Errors);
        }

        var user = await _userRepository.GetByEmailAsync(emailResult.Value);
        if (user is null)
        {
            var error = ResultError.NullValue("UserId", $"User with email '{email}' not found.");
            return ResultT<UserDomain>.FailureT(ResultType.NotFound, error);
        }

        if (!user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation(
                "User",
                $"The user with email '{email}' has not verified their email address. Password reset is only allowed for users with verified email addresses.");
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);
        }

        return user;
    }
}
