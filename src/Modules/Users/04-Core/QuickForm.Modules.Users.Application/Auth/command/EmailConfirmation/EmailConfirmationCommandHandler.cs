using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

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
            return ResultT<ResultResponse>.FailureT(authActionTokenResult.ResultType,authActionTokenResult.Errors);
        }

        var authActionToken = authActionTokenResult.Value;
        var userDomainResult = await ValidateInputData(authActionToken.IdUser);
        if (userDomainResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(userDomainResult.ResultType,userDomainResult.Errors);
        }
        var user = userDomainResult.Value;
        user.ConfirmEmail();
        authActionToken.UseToken();

        _unitOfWork.Repository<UserDomain,UserId>().UpdateEntity(user);

        _unitOfWork.Repository<AuthActionTokenDomain, AuthActionTokenId>().UpdateEntity(authActionToken);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }
        return ResultResponse.Success("Email confirmed successfully.");

    }
    private async Task<ResultT<AuthActionTokenDomain>> GetAuthActionTokenByToken(string token)
    {
        var idAuthActionEmailVerificacion = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new MasterId(idAuthActionEmailVerificacion);
        var tokenResult = TokenVO.Create(token);
        if (tokenResult.IsFailure)
        {
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation, tokenResult.Errors);
        }
        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdAndTokenAsync(idAuthAction, tokenResult.Value);
        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                $"The email confirmation token '{token}' could not be found. Please ensure that you have entered a valid token."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.NotFound, error);
        }

        if (authActionToken.Used)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The email confirmation token '{token}' has already been used. Please request a new token if needed."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation, error);
        }

        if (authActionToken.ExpiresAt.Value <= _dateTimeProvider.UtcNow)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                $"The email confirmation token '{token}' has expired. Please request a new token to proceed with the confirmation."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.MismatchValidation,error);
        }

        return authActionToken;
    }

    private async Task<ResultT<UserDomain>> ValidateInputData(UserId userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            var error = ResultError.NullValue("UserId", $"User with id '{userId}' not found.");
            return ResultT<UserDomain>.FailureT(ResultType.NotFound, error);
        }
        if (user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation("Email", $"The email associated with the user ID '{userId}' has already been verified. No further action is required.");

            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);

        }
        return user;
    }

}
