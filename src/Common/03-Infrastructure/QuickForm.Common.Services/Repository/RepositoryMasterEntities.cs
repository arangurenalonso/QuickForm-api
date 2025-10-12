
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class RepositoryMasterEntities<TEntity>
        : RepositoryBase<TEntity, MasterId>, IRepositoryMasterEntities<TEntity>
        where TEntity : BaseMasterEntity
{
    public RepositoryMasterEntities(DbContext context) : base(context) { }


    public async Task<FindResult<TEntity, KeyNameVO>> GetByKeyNames(
            List<KeyNameVO> keyNames,
            bool asNoTracking,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        )
    {
        var needleValues = keyNames.Select(k => k.Value).ToList();
        var query = _context.Set<TEntity>().Where(x => needleValues.Contains(x.KeyName.Value) && !x.IsDeleted);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        if (predicado is not null)
        {
            query = query.Where(predicado);
        }
        var found = await query.ToListAsync(cancellationToken);
        var result = FindResult<TEntity, KeyNameVO>.From(
                        found: found,
                        requested: keyNames,
                        foundKeySelector: e => e.KeyName
                    );

        return result;
    }

    public async Task<TEntity?> GetByKeyName(
            KeyNameVO keyName,
            bool asNoTracking,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        )
    {
        var query = _context.Set<TEntity>().Where(x => x.KeyName == keyName && !x.IsDeleted);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        if (predicado is not null)
        {
            query = query.Where(predicado);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<TEntity?> GetFromIdOrKeyName(
        IdOrKeyName? idOrKeyName, 
        bool asNoTracking, 
        Expression<Func<TEntity, bool>>? predicado = null, 
        CancellationToken ct = default
        )
    {
        if (idOrKeyName is null)
        {
            return null;
        }
        if (idOrKeyName.IsById)
        {
            return await GetById(idOrKeyName.Id, asNoTracking, ct);
        }

        if (idOrKeyName.IsByKeyName)
        {
            return await GetByKeyName(idOrKeyName.Name, asNoTracking, predicado, ct);
        }

        throw new InvalidOperationException("Referencia de aplicación inválida.");
    }
    public async Task<FindResult<MasterEntityDto, KeyNameVO>> GetDtoByKeyNames(
            List<KeyNameVO> keyNames,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        )
    {
        var query = _context
                        .Set<TEntity>()
                        .AsNoTracking()
                        .Where(x => keyNames.Contains(x.KeyName) && !x.IsDeleted);
        
        if (predicado is not null)
        {
            query = query.Where(predicado);
        }

        var queryDto = query.Select(x => new MasterEntityDto()
                   {
                       Id = x.Id,
                       KeyName = x.KeyName,
                       Description = x.Description
                   });
        var found = await queryDto.ToListAsync(cancellationToken);
        var result = FindResult<MasterEntityDto, KeyNameVO>.From(
                        found: found,
                        requested: keyNames,
                        foundKeySelector: e => e.KeyName
                    );

        return result;
    }

    public async Task<MasterEntityDto?> GetDtoById(
               MasterId id,
               CancellationToken cancellationToken = default
           )
    {
        return await _context.Set<TEntity>()
                        .Where(x => x.Id == id && !x.IsDeleted)
                        .Select(x => new MasterEntityDto()
                        {
                            Id = x.Id,
                            KeyName = x.KeyName,
                            Description = x.Description
                        }
                        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MasterEntityDto?> GetDtotByKeyName(
               KeyNameVO keyName,
               Expression<Func<TEntity, bool>>? predicado = null,
               CancellationToken cancellationToken = default
           )
    {
        var query = _context.Set<TEntity>()
                            .Where(x => x.KeyName == keyName && !x.IsDeleted);

        if (predicado is not null)
        {
            query = query.Where(predicado);
        }


        return await query
                        .Select(x => new MasterEntityDto()
                        {
                            Id = x.Id,
                            KeyName = x.KeyName,
                            Description = x.Description
                        }
                        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MasterEntityDto?> GetDtoFromIdOrKeyName(
        IdOrKeyName? idOrKeyName, 
        Expression<Func<TEntity, bool>>? predicado = null,
        CancellationToken ct = default
        )
    {
        if (idOrKeyName is null)
        {
            return null;
        }

        if (idOrKeyName.IsById)
        {
            return await GetDtoById(idOrKeyName.Id, ct);
        }

        if (idOrKeyName.IsByKeyName)
        {
            return await GetDtotByKeyName(idOrKeyName.Name, predicado, ct);
        }

        throw new InvalidOperationException("Referencia de aplicación inválida.");
    }

}
