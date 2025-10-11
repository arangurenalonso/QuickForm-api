
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Infrastructure;
public class RepositoryMasterEntities<TEntity>
        : RepositoryBase<TEntity, MasterId>, IRepositoryMasterEntities<TEntity>
        where TEntity : BaseMasterEntity
{
    public RepositoryMasterEntities(DbContext context) : base(context) { }

    public async Task<TEntity?> GetById(
               Guid id,
               bool asNoTracking,
               CancellationToken cancellationToken = default
           )
    {
        var masterId = new MasterId(id);    
        return await GetById(masterId, asNoTracking, cancellationToken);
    }
    public async Task<TEntity?> GetByKeyName(
            string keyName,
            bool asNoTracking,
            CancellationToken cancellationToken = default
        )
    {
        var keyNameResult = KeyNameVO.Create(keyName);
        if (keyNameResult.IsFailure)
        {
            return null;
        }
        return await GetByKeyName(keyNameResult.Value, asNoTracking, cancellationToken);
    }
    public async Task<TEntity?> GetByKeyName(
            KeyNameVO keyName,
            bool asNoTracking,
            CancellationToken cancellationToken = default
        )
    {
        var query = _context.Set<TEntity>().Where(x => x.KeyName == keyName && !x.IsDeleted);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<TEntity?> GetFromIdOrKeyName(IdOrKeyName? aplicacionRef, bool asNoTracking, CancellationToken ct)
    {
        if (aplicacionRef is null)
        {
            return null;
        }
        if (aplicacionRef.IsById)
        {
            return await GetById(aplicacionRef.Id.Value, asNoTracking, ct);
        }

        if (aplicacionRef.IsByKeyName)
        {
            return await GetByKeyName(aplicacionRef.Name, asNoTracking, ct);
        }

        throw new InvalidOperationException("Referencia de aplicación inválida.");
    }
    
    public async Task<MasterEntityDto?> GetDtoById(
               Guid id,
               CancellationToken cancellationToken = default
           )
    {
        var masterId = new MasterId(id);
        return await GetDtoById(masterId,cancellationToken);
    }

    public async Task<MasterEntityDto?> GetDtoById(
               MasterId id,
               CancellationToken cancellationToken = default
           )
    {
        return await _context.Set<TEntity>()
                        .AsNoTracking()
                        .Where(x => x.Id == id && !x.IsDeleted)
                        .Select(x => new MasterEntityDto()
                        {
                            Id = x.Id.Value,
                            KeyName = x.KeyName.Value,
                            Description = x.Description != null ? x.Description.Value : null
                        }
                        ).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<MasterEntityDto?> GetDtotByKeyName(
               string keyNameString,
               CancellationToken cancellationToken = default
           )
    {
        var keyNameResult = KeyNameVO.Create(keyNameString);
        if (keyNameResult.IsFailure)
        {
            return null;
        }
        KeyNameVO keyName = keyNameResult.Value;
        return await GetDtotByKeyName(keyName, cancellationToken);
    }

    public async Task<MasterEntityDto?> GetDtotByKeyName(
               KeyNameVO keyName,
               CancellationToken cancellationToken = default
           )
    {
        return await _context.Set<TEntity>()
                        .AsNoTracking()
                        .Where(x => x.KeyName == keyName && !x.IsDeleted)
                        .Select(x => new MasterEntityDto()
                        {
                            Id = x.Id.Value,
                            KeyName = x.KeyName.Value,
                            Description = x.Description != null ? x.Description.Value : null
                        }
                        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MasterEntityDto?> GetDtoFromIdOrKeyName(IdOrKeyName? aplicacionRef, CancellationToken ct)
    {
        if (aplicacionRef is null)
        {
            return null;
        }
        if (aplicacionRef.IsById)
        {
            return await GetDtoById(aplicacionRef.Id.Value, ct);
        }

        if (aplicacionRef.IsByKeyName)
        {
            return await GetDtotByKeyName(aplicacionRef.Name, ct);
        }

        throw new InvalidOperationException("Referencia de aplicación inválida.");
    }

}
