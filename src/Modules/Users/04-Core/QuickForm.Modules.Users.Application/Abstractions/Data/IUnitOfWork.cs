using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(string classOrigin, CancellationToken cancellationToken = default);
    IGenericUserRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId;
    IGenericUserMasterEntityRepository<TEntity> Repository<TEntity>()
     where TEntity : BaseMasterEntity;
}
