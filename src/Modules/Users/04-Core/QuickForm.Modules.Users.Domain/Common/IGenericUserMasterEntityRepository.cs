using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public interface IGenericUserMasterEntityRepository<TEntity> : IRepositoryMasterEntities<TEntity>
    where TEntity : BaseMasterEntity
{
}

