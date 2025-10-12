using System.Linq.Expressions;

namespace QuickForm.Common.Domain;
public interface IRepositoryMasterEntities<TEntity>
        : IRepositoryBase<TEntity, MasterId>
        where TEntity : BaseMasterEntity
{
    Task<FindResult<TEntity, KeyNameVO>> GetByKeyNames(
            List<KeyNameVO> keyNames,
            bool asNoTracking,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        );

    Task<TEntity?> GetByKeyName( 
            KeyNameVO keyName,
            bool asNoTracking,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        );
    Task<TEntity?>  GetFromIdOrKeyName(
        IdOrKeyName? idOrKeyName,
        bool asNoTracking, 
        Expression<Func<TEntity, bool>>? predicado = null,
        CancellationToken ct = default
        );
    Task<FindResult<MasterEntityDto, KeyNameVO>> GetDtoByKeyNames(
            List<KeyNameVO> keyNames,
            Expression<Func<TEntity, bool>>? predicado = null,
            CancellationToken cancellationToken = default
        );

    Task<MasterEntityDto?> GetDtoById(
               MasterId id,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtotByKeyName(
               KeyNameVO keyName,
               Expression<Func<TEntity, bool>>? predicado = null,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtoFromIdOrKeyName(
            IdOrKeyName? idOrKeyName, 
            Expression<Func<TEntity, bool>>? predicado = null, 
            CancellationToken ct = default
        );
}
