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
    IUnitOfWork _unitOfWork,
    IRefreshTokenService _refreshTokenService,
    IAuthActionTokenHashingService _authActionTokenHashingService
) : ICommandHandler<EmailConfirmationCommand, AuthSessionResponse>
{
    public async Task<ResultT<AuthSessionResponse>> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var userDomainResult = await GetUserByEmail(request.Email);
        if (userDomainResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(userDomainResult.ResultType, userDomainResult.Errors);
        }

        var user = userDomainResult.Value;

        var authActionTokenResult = await GetAuthActionTokenByToken(user.Email, request.Token);
        if (authActionTokenResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(authActionTokenResult.ResultType, authActionTokenResult.Errors);
        }

        var authActionToken = authActionTokenResult.Value;

        user.ConfirmEmail();

        var useTokenResult = authActionToken.UseToken(_dateTimeProvider.UtcNow);
        if (useTokenResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(useTokenResult.ResultType, useTokenResult.Errors);
        }

        _unitOfWork.Repository<UserDomain, UserId>().UpdateEntity(user);
        _unitOfWork.Repository<AuthActionTokenDomain, AuthActionTokenId>().UpdateEntity(authActionToken);

        var sessionResult = await CreateSessionAsync(user, cancellationToken);
        if (sessionResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(sessionResult.ResultType, sessionResult.Errors);
        }

        return sessionResult.Value;
    }

    private async Task<ResultT<AuthActionTokenDomain>> GetAuthActionTokenByToken(EmailVO email, string token)
    {
        var tokenHashResult = _authActionTokenHashingService.Hash(token);
        if (tokenHashResult.IsFailure)
        {
            return ResultT<AuthActionTokenDomain>.FailureT(tokenHashResult.ResultType, tokenHashResult.Errors);
        }

        var idAuthAction = new MasterId(AuthActionType.EmailConfirmation.GetId());

        var authActionToken = await _authActionTokenRepository.GetAuthActionTokenByAuthActionIdEmailAndTokenHashAsync(
            idAuthAction,
            email,
            tokenHashResult.Value);

        if (authActionToken is null)
        {
            var error = ResultError.NullValue(
                "AuthActionToken",
                "The email confirmation token could not be found. Please ensure that you have entered a valid token."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.NotFound, error);
        }

        if (authActionToken.Used)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                "The email confirmation token has already been used. Please request a new token if needed."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.DomainValidation, error);
        }

        if (authActionToken.ExpiresAt.Value <= _dateTimeProvider.UtcNow)
        {
            var error = ResultError.InvalidOperation(
                "AuthActionToken",
                "The email confirmation token has expired. Please request a new token to proceed with the confirmation."
            );
            return ResultT<AuthActionTokenDomain>.FailureT(ResultType.MismatchValidation, error);
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
            var error = ResultError.InvalidOperation("Email", $"The email associated with '{email}' has already been verified.");
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);
        }

        return user;
    }

    private async Task<ResultT<AuthSessionResponse>> CreateSessionAsync(
        UserDomain user,
        CancellationToken cancellationToken)
    {
        try
        {
            var accessTokenResult = _tokenService.GenerateToken(user.Id.Value, user.Email.Value);

            if (accessTokenResult.IsFailure)
            {
                return ResultT<AuthSessionResponse>.FailureT(accessTokenResult.ResultType, accessTokenResult.Errors);
            }

            var refreshTokenResult = RefreshTokenDomain.Create(
                user.Id,
                _dateTimeProvider.UtcNow,
                _refreshTokenService);

            if (refreshTokenResult.IsFailure)
            {
                return ResultT<AuthSessionResponse>.FailureT(refreshTokenResult.ResultType, refreshTokenResult.Errors);
            }

            var refreshToken = refreshTokenResult.Value;

            _unitOfWork.Repository<RefreshTokenDomain, RefreshTokenId>().AddEntity(refreshToken);

            var saveResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
            if (saveResult.IsFailure)
            {
                return ResultT<AuthSessionResponse>.FailureT(saveResult.ResultType, saveResult.Errors);
            }

            var response = new AuthSessionResponse(
                accessTokenResult.Value,
                refreshToken.PlainTextToken!,
                new UserResponse(user.Id.Value, user.Email.Value)
            );

            refreshToken.ClearPlainTextToken();

            return response;
        }
        catch (Exception ex)
        {
            return CommonMethods.ConvertExceptionToResult(ex, "AuthSession");
        }
    }
}
