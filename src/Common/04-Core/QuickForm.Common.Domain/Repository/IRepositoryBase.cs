namespace QuickForm.Common.Domain;

public interface IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
    void AddEntity(TEntity entity);
    void AddEntity(List<TEntity> entities);
    void UpdateEntity(TEntity entity);
    void UpdateEntity(IEnumerable<TEntity> entities);
    void DeleteEntity(TEntity entity);
    void DeleteEntity(List<TEntity> entities);
}
