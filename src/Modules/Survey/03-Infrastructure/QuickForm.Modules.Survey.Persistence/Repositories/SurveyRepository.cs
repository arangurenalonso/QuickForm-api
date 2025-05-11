using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;

namespace QuickForm.Modules.Survey.Persistence;
public class SurveyRepository<TEntity, TEntityId> : RepositoryBase<TEntity, TEntityId>
    where TEntity : BaseDomainEntity<TEntityId>
    where TEntityId : EntityId
{
    public SurveyRepository(SurveyDbContext context) : base(context) { }
}
