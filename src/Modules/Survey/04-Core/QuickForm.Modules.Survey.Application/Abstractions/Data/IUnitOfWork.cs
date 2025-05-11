using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(string classOrigin, CancellationToken cancellationToken = default);
    ISurveyRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
     where TEntity : BaseDomainEntity<TEntityId>
     where TEntityId : EntityId;
}
