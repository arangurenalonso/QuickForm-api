using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class RepositoryBase<TEntity,TEntityId>: IRepositoryBase<TEntity, TEntityId>
    where TEntityId : EntityId
    where TEntity : BaseDomainEntity<TEntityId>
{
    protected readonly DbContext _context;

    public RepositoryBase(DbContext context)
    {
        _context = context;
    }

    public async Task<TEntity> GetById(TEntityId id,
               bool asNoTracking,
               CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().Where(x => x.Id == id && !x.IsDeleted);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<TEntity> GetAll(
               bool asNoTracking,
               CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().Where(x => !x.IsDeleted);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
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

    public async Task<bool> ExistEntity(TEntityId entityId)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.Id == entityId && e.IsDeleted);
    }
}
