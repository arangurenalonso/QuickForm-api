using Microsoft.Extensions.Options;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class LoginCommandHandler(
    IPasswordHashingService _passwordHashingService,
    ITokenService _tokenService,
    IDateTimeProvider _dateTimeProvider,
    IUserRepository _userRepository,
    IRefreshTokenService _refreshTokenService,
    IUnitOfWork _unitOfWork
) : ICommandHandler<LoginCommand, AuthSessionResponse>
{
    public async Task<ResultT<AuthSessionResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var resultUser = await ValidateInputData(request.Email);
        if (resultUser.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(resultUser.ResultType, resultUser.Errors);
        }

        var user = resultUser.Value;

        var resultPassword = ValidatePassword(user, request.Password);
        if (resultPassword.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(resultPassword.ResultType, resultPassword.Errors);
        }

        var sessionResult = await CreateSessionAsync(user, cancellationToken);
        if (sessionResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(sessionResult.ResultType, sessionResult.Errors);
        }

        return sessionResult.Value;
    }

    private async Task<ResultT<UserDomain>> ValidateInputData(string email)
    {
        var emailVO = EmailVO.Create(email);
        if (emailVO.IsFailure)
        {
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, emailVO.Errors);
        }

        var user = await _userRepository.GetByEmailAsync(emailVO.Value);
        if (user is null)
        {
            var error = ResultError.NullValue("Email", "Invalid email or password.");
            return ResultT<UserDomain>.FailureT(ResultType.NotFound, error);
        }

        if (!user.IsEmailVerify)
        {
            var error = ResultError.InvalidOperation("Email", "Email has not been verified. Please verify your email to proceed.");
            error.SetRedirectUrl($"/auth/resend-verification?email={email}");
            return ResultT<UserDomain>.FailureT(ResultType.DomainValidation, error);
        }

        return user;
    }

    private Result ValidatePassword(UserDomain user, string password)
    {
        var result = _passwordHashingService.VerifyPassword(password, user.PasswordHash.Value);

        if (!result)
        {
            var error = ResultError.InvalidOperation("Password", "Invalid email or password.");
            return Result.Failure(ResultType.MismatchValidation, error);
        }

        return Result.Success();
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
