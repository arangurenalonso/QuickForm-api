using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class SurveryMasterRepository<TEntity>
    : RepositoryMasterEntities<TEntity>, ISurveryMasterRepository<TEntity>
    where TEntity : BaseMasterEntity
{
    public SurveryMasterRepository(SurveyDbContext context) : base(context) { }
}

