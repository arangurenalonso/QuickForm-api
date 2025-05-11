using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public interface ISurveyRepository<TEntity, TEntityId> : IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
}
