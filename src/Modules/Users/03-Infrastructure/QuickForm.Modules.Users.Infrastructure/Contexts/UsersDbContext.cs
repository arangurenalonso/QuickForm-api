using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Application;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUnitOfWork
{
    public required DbSet<UserDomain> Users { get; set; }
    public required DbSet<AuthActionDomain> AuthAction { get; set; }
    public required DbSet<AuthActionTokenDomain> AuthActionToken { get; set; }
    public required DbSet<AuditLog> Audit { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Auth);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        base.OnModelCreating(modelBuilder);
    }
    public async Task<ResultT<int>> SaveChangesWithResultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return ResultT<int>.Success(result);
        }
        catch (Exception e)
        {
            var listResultError= CommonMethods.ConvertExceptionToResult(e, "Database Transaction");
            return ResultT<int>.Failure(ResultType.DataBaseTransaction,listResultError);
        }
    }

}
