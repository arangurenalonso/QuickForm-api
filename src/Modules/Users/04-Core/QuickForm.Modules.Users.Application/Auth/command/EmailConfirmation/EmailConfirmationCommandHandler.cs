using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Domain.AuthActionToken.Enum;

namespace QuickForm.Modules.Users.Application;
public class EmailConfirmationCommandHandler(
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IUserRepository _userRepository,
    IAuthActionTokenRepository _authActionTokenRepository
    ) : ICommandHandler<EmailConfirmationCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var authActionTokenResult = await GetAuthActionTokenByToken(request.Token);
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
        user.ConfirmEmail();
        authActionToken.UseToken();

        _userRepository.Update(user);
        _authActionTokenRepository.Update(authActionToken);

        var result = await _unitOfWork.SaveChangesWithResultAsync(cancellationToken);
        if (result.IsFailure)
        {
            return ResultT<ResultResponse>.Failure(result.ResultType,result.Errors);
        }
        return ResultResponse.Success("Email confirmed successfully.");

    }
    private async Task<ResultT<AuthActionTokenDomain>> GetAuthActionTokenByToken(string token)
    {
        var idAuthActionEmailVerificacion = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new AuthActionId(idAuthActionEmailVerificacion);
        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdAndTokenAsync(idAuthAction, token);
        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                $"The email confirmation token '{token}' could not be found. Please ensure that you have entered a valid token."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.NotFound, error);
        }

        if (authActionToken.Used)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The email confirmation token '{token}' has already been used. Please request a new token if needed."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.DomainValidation, error);
        }

        if (authActionToken.ExpiresAt.Value <= _dateTimeProvider.UtcNow)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The email confirmation token '{token}' has expired. Please request a new token to proceed with the confirmation."
            );
            return ResultT<AuthActionTokenDomain>.Failure(ResultType.MismatchValidation,error);
        }

        return authActionToken;
    }

    private async Task<ResultT<UserDomain>> ValidateInputData(UserId userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            var error = ResultError.NullValue("UserId", $"User with id '{userId}' not found.");
            return ResultT<UserDomain>.Failure(ResultType.NotFound, error);
        }
        if (user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation("Email", $"The email associated with the user ID '{userId}' has already been verified. No further action is required.");

            return ResultT<UserDomain>.Failure(ResultType.DomainValidation, error);

        }
        return user;
    }

}
