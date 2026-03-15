using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public class LogoutCommandHandler(
    IRefreshTokenRepository _refreshTokenRepository,
    IRefreshTokenService _refreshTokenService,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider
) : ICommandHandler<LogoutCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ResultResponse.Success("Logged out successfully.");
        }

        var hashResult = _refreshTokenService.Hash(request.RefreshToken);
        if (hashResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(hashResult.ResultType, hashResult.Errors);
        }

        var token = await _refreshTokenRepository.GetByTokenHashWithUserAsync(hashResult.Value, cancellationToken);

        if (token is null)
        {
            return ResultResponse.Success("Logged out successfully.");
        }

        token.Revoke(_dateTimeProvider.UtcNow);

        var saveResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (saveResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(saveResult.ResultType, saveResult.Errors);
        }

        return ResultResponse.Success("Logged out successfully.");
    }
}
