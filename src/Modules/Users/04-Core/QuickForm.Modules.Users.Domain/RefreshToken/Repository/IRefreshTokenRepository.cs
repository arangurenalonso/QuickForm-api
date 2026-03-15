
namespace QuickForm.Modules.Users.Domain;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenDomain?> GetByTokenHashWithUserAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<List<RefreshTokenDomain>> GetActiveByUserAndFamilyAsync(
        UserId userId,
        Guid familyId,
        CancellationToken cancellationToken = default);
}
