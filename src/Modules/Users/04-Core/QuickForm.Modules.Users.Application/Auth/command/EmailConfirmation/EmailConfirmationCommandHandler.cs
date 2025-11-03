using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class EmailConfirmationCommandHandler(
        IAuthActionTokenRepository _authActionTokenRepository,
        IDateTimeProvider _dateTimeProvider,
        IUserRepository _userRepository,
        ITokenService _tokenService,
        IUnitOfWork _unitOfWork
    ) : ICommandHandler<EmailConfirmationCommand, string>
{

    public async Task<ResultT<string>> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {

        var userDomainResult = await GetUserByEmail(request.Email);
        if (userDomainResult.IsFailure)
        {
            return ResultT<string>.FailureT(userDomainResult.ResultType, userDomainResult.Errors);
        }
        var user = userDomainResult.Value;

        var authActionTokenResult = await GetAuthActionTokenByToken(user.Email, request.Token);
        if (authActionTokenResult.IsFailure)
        {
            return ResultT<string>.FailureT(authActionTokenResult.ResultType,authActionTokenResult.Errors);
        }

        var authActionToken = authActionTokenResult.Value;
        if (userDomainResult.IsFailure)
        {
            return ResultT<string>.FailureT(userDomainResult.ResultType,userDomainResult.Errors);
        }

        user.ConfirmEmail();
        authActionToken.UseToken();

        _unitOfWork.Repository<UserDomain,UserId>().UpdateEntity(user);
        _unitOfWork.Repository<AuthActionTokenDomain, AuthActionTokenId>().UpdateEntity(authActionToken);

        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<string>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }


        return CreateAuthenticationResult(user);

    }
    private async Task<ResultT<AuthActionTokenDomain>> GetAuthActionTokenByToken(EmailVO email,string token)
    {
        var idAuthActionEmailVerificacion = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new MasterId(idAuthActionEmailVerificacion);
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
        if (user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation("Email", $"The email associated with the user ID '{email}' has already been verified. No further action is required.");

            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);

        }
        return user;
    }
    private ResultT<string> CreateAuthenticationResult(UserDomain user)
    {
        try
        {
            var resultTokenGenerate = _tokenService.GenerateToken(user.Id.Value, user.Email.Value);

            if (resultTokenGenerate.IsFailure)
            {
                return ResultT<string>.FailureT(resultTokenGenerate.ResultType, resultTokenGenerate.Errors);
            }
            return resultTokenGenerate.Value;
        }
        catch (Exception e)
        {
            return CommonMethods.ConvertExceptionToResult(e, "Token");
        }
    }

}
