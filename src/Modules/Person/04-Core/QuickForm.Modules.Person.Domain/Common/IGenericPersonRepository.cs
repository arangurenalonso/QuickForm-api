using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public interface IGenericPersonRepository<TEntity, TEntityId> : IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
}
