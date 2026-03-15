using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence.Repositories;

public class RefreshTokenRepository(
    UsersDbContext _context
) : IRefreshTokenRepository
{
    public async Task<RefreshTokenDomain?> GetByTokenHashWithUserAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<RefreshTokenDomain>()
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<List<RefreshTokenDomain>> GetActiveByUserAndFamilyAsync(
        UserId userId,
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Set<RefreshTokenDomain>()
            .Where(x =>
                x.IdUser == userId &&
                x.FamilyId == familyId &&
                x.RevokedAt == null &&
                x.ExpiresAt > now &&
                !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
