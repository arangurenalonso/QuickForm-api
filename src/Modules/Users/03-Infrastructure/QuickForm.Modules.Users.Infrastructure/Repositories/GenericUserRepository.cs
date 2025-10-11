using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class GenericUserRepository<TEntity, TEntityId>
    : RepositoryBase<TEntity, TEntityId>, IGenericUserRepository<TEntity, TEntityId>
    where TEntity : BaseDomainEntity<TEntityId>
    where TEntityId : EntityId
{
    public GenericUserRepository(UsersDbContext context) : base(context) { }
}
