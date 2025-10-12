using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public interface ISurveryMasterRepository<TEntity> : IRepositoryMasterEntities<TEntity>
    where TEntity : BaseMasterEntity
{
}

