using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class RepositoryBase<TEntity,TEntityId>(
        DbContext _context
    ) : IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
    public void AddEntity(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }
    public void AddEntity(List<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
    }
    public void UpdateEntity(TEntity entity)
    {
        _context.Set<TEntity>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
    public void UpdateEntity(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().UpdateRange(entities);
    }
    public void DeleteEntity(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }
    public void DeleteEntity(List<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
    }


}
