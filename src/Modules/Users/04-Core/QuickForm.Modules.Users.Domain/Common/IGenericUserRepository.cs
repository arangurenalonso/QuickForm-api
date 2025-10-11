using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public interface IGenericUserRepository<TEntity, TEntityId> : IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
}
