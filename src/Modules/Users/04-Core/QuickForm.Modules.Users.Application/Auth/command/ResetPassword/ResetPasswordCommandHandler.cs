using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

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

        var userDomainResult = await GetUserByEmail(request.Email);
        if (userDomainResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(userDomainResult.ResultType,userDomainResult.Errors);
        }
        var user = userDomainResult.Value;


        var authActionTokenResult = await GetUserByAuthActionToken(user.Email,request.Token);
        if (authActionTokenResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(authActionTokenResult.ResultType,authActionTokenResult.Errors);
        }
        var authActionToken = authActionTokenResult.Value;

        user.ChangePassword(request.Password, _passwordHashingService);
        authActionToken.UseToken();

        _unitOfWork.Repository<UserDomain, UserId>().UpdateEntity(user);
        _unitOfWork.Repository<AuthActionTokenDomain, AuthActionTokenId>().UpdateEntity(authActionToken);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }
        return ResultResponse.Success("Your password has been reset successfully.");

    }
    private async Task<ResultT<AuthActionTokenDomain>> GetUserByAuthActionToken(EmailVO email, string token)
    {
        var idAuthActionRecoveryPassword = AuthActionType.RecoveryPassword.GetId();
        var idAuthAction = new MasterId(idAuthActionRecoveryPassword);

        var tokenResult = TokenVO.Create(token);
        if (tokenResult.IsFailure)
        {
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation, tokenResult.Errors);
        }

        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdEmailAndTokenAsync(
                                                                        idAuthAction,
                                                                        email,
                                                                        tokenResult.Value);
        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                $"The password recovery token '{token}' could not be found. Please ensure that you have entered a valid token."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.NotFound,error);
        }

        if (authActionToken.Used)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The password recovery token '{token}' has already been used. Please request a new token if needed."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation, error);
        }

        if (authActionToken.ExpiresAt.Value <= _dateTimeProvider.UtcNow)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The password recovery token '{token}' has expired. Please request a new token to proceed with the confirmation."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation,error);
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
                $"The user with email '{email}' has not verified their email address. Password reset is only allowed for users with verified email addresses."
            );

            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);
        }
        return user;
    }
}
