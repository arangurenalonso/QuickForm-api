using System.ComponentModel.DataAnnotations.Schema;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public class RefreshTokenDomain : BaseDomainEntity<RefreshTokenId>
{
    public UserId IdUser { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public Guid FamilyId { get; private set; }
    public string? CreatedByIp { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? UserAgent { get; private set; }

    [NotMapped]
    public string? PlainTextToken { get; private set; }

    #region One To Many Relationship
    public UserDomain User { get; private set; } = null!;
    #endregion

    private RefreshTokenDomain() { }

    private RefreshTokenDomain(
        RefreshTokenId id,
        UserId idUser,
        string tokenHash,
        DateTime expiresAt,
        Guid familyId,
        string? plainTextToken,
        string? createdByIp,
        string? userAgent
    ) : base(id)
    {
        IdUser = idUser;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        FamilyId = familyId;
        PlainTextToken = plainTextToken;
        CreatedByIp = createdByIp;
        UserAgent = userAgent;
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public bool IsActive(DateTime now) => RevokedAt is null && !IsExpired(now);

    public static ResultT<RefreshTokenDomain> Create(
        UserId userId,
        DateTime now,
        IRefreshTokenService refreshTokenService,
        Guid? familyId = null,
        string? createdByIp = null,
        string? userAgent = null)
    {
        var expirationDays = refreshTokenService.ExpirationDays();
        if (expirationDays <= 0)
        {
            return ResultError.InvalidInput("RefreshToken", "ExpirationDays must be greater than zero.");
        }

        var plainToken = refreshTokenService.Generate();

        if (string.IsNullOrWhiteSpace(plainToken))
        {
            return ResultError.InvalidOperation("RefreshToken", "Refresh token generator returned an empty token.");
        }

        var hashResult = refreshTokenService.Hash(plainToken);
        if (hashResult.IsFailure)
        {
            return hashResult.Errors;
        }

        return new RefreshTokenDomain(
            RefreshTokenId.Create(),
            userId,
            hashResult.Value,
            now.AddDays(expirationDays),
            familyId ?? Guid.NewGuid(),
            plainToken,
            createdByIp,
            userAgent
        );
    }

    public Result ReplaceWith(RefreshTokenDomain newToken, DateTime now, string? revokedByIp = null)
    {
        if (newToken is null)
        {
            return ResultError.NullValue("RefreshToken", "Replacement token cannot be null.");
        }

        if (RevokedAt is not null)
        {
            return ResultError.InvalidOperation("RefreshToken", "Refresh token has already been revoked.");
        }

        RevokedAt = now;
        ReplacedByTokenId = newToken.Id.Value;
        RevokedByIp = revokedByIp;

        return Result.Success();
    }

    public Result Revoke(DateTime now, string? revokedByIp = null)
    {
        if (RevokedAt is not null)
        {
            return Result.Success();
        }

        RevokedAt = now;
        RevokedByIp = revokedByIp;

        return Result.Success();
    }

    public void ClearPlainTextToken()
    {
        PlainTextToken = null;
    }
}
