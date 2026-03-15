using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository _refreshTokenRepository,
    IRefreshTokenService _refreshTokenService,
    ITokenService _tokenService,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider
) : ICommandHandler<RefreshTokenCommand, AuthSessionResponse>
{
    public async Task<ResultT<AuthSessionResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var error = ResultError.EmptyValue("RefreshToken", "Refresh token cannot be null or empty.");
            return ResultT<AuthSessionResponse>.FailureT(ResultType.DomainValidation, error);
        }

        var hashResult = _refreshTokenService.Hash(request.RefreshToken);
        if (hashResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(hashResult.ResultType, hashResult.Errors);
        }

        var currentToken = await _refreshTokenRepository.GetByTokenHashWithUserAsync(hashResult.Value, cancellationToken);
        if (currentToken is null)
        {
            var error = ResultError.InvalidOperation("RefreshToken", "Invalid refresh token.");
            return ResultT<AuthSessionResponse>.FailureT(ResultType.MismatchValidation, error);
        }

        var now = _dateTimeProvider.UtcNow;

        if (currentToken.RevokedAt is not null)
        {
            var familyTokens = await _refreshTokenRepository.GetActiveByUserAndFamilyAsync(currentToken.IdUser, currentToken.FamilyId, cancellationToken);

            foreach (var token in familyTokens)
            {
                token.Revoke(now);
            }

            await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);

            var error = ResultError.InvalidOperation("RefreshToken", "Refresh token has already been revoked.");
            return ResultT<AuthSessionResponse>.FailureT(ResultType.MismatchValidation, error);
        }

        if (currentToken.IsExpired(now))
        {
            currentToken.Revoke(now);
            await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);

            var error = ResultError.InvalidOperation("RefreshToken", "Refresh token has expired.");
            return ResultT<AuthSessionResponse>.FailureT(ResultType.MismatchValidation, error);
        }

        var newRefreshTokenResult = RefreshTokenDomain.Create(
            currentToken.IdUser,
            now,
            _refreshTokenService,
            currentToken.FamilyId);

        if (newRefreshTokenResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(newRefreshTokenResult.ResultType, newRefreshTokenResult.Errors);
        }

        var newRefreshToken = newRefreshTokenResult.Value;

        var replaceResult = currentToken.ReplaceWith(newRefreshToken, now);
        if (replaceResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(replaceResult.ResultType, replaceResult.Errors);
        }

        var accessTokenResult = _tokenService.GenerateToken(
            currentToken.User.Id.Value,
            currentToken.User.Email.Value);

        if (accessTokenResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(accessTokenResult.ResultType, accessTokenResult.Errors);
        }

        _unitOfWork.Repository<RefreshTokenDomain, RefreshTokenId>().AddEntity(newRefreshToken);

        var saveResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (saveResult.IsFailure)
        {
            return ResultT<AuthSessionResponse>.FailureT(saveResult.ResultType, saveResult.Errors);
        }

        var response = new AuthSessionResponse(
            accessTokenResult.Value,
            newRefreshToken.PlainTextToken!,
            new UserResponse(currentToken.User.Id.Value, currentToken.User.Email.Value)
        );

        newRefreshToken.ClearPlainTextToken();

        return response;
    }
}
