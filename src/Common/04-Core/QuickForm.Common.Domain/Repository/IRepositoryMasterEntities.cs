namespace QuickForm.Common.Domain;
public interface IRepositoryMasterEntities<TEntity>
        : IRepositoryBase<TEntity, MasterId>
        where TEntity : BaseMasterEntity
{

}
