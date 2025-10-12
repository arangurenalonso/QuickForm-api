using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public interface IGenericMasterEntityPersonRepository<TEntity> : IRepositoryMasterEntities<TEntity>
    where TEntity : BaseMasterEntity
{
}

