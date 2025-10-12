using QuickForm.Common.Domain;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(string classOrigin, CancellationToken cancellationToken = default);
    IGenericPersonRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId;
    IGenericMasterEntityPersonRepository<TEntity> Repository<TEntity>()
     where TEntity : BaseMasterEntity;
}
