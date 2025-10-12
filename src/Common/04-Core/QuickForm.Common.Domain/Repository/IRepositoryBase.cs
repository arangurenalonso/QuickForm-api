namespace QuickForm.Common.Domain;

public interface IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
    Task<TEntity> GetById(TEntityId id,
               bool asNoTracking,
               CancellationToken cancellationToken = default
        );
    Task<TEntity> GetAll(
               bool asNoTracking,
               CancellationToken cancellationToken = default);
    void AddEntity(TEntity entity);
    void AddEntity(List<TEntity> entities);
    void UpdateEntity(TEntity entity);
    void UpdateEntity(IEnumerable<TEntity> entities);
    void DeleteEntity(TEntity entity);
    void DeleteEntity(List<TEntity> entities);
    Task<bool> ExistEntity(TEntityId entityId);
}
