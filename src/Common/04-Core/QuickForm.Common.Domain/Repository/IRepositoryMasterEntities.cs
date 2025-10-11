namespace QuickForm.Common.Domain;
public interface IRepositoryMasterEntities<TEntity>
        : IRepositoryBase<TEntity, MasterId>
        where TEntity : BaseMasterEntity
{
    Task<TEntity?> GetById(
               Guid id,
               bool asNoTracking,
               CancellationToken cancellationToken = default
           );
    Task<TEntity?> GetByKeyName(
            string keyName,
            bool asNoTracking,
            CancellationToken cancellationToken = default
        );
    Task<TEntity?> GetByKeyName(
            KeyNameVO keyName,
            bool asNoTracking,
            CancellationToken cancellationToken = default
        );
    Task<TEntity?>  GetFromIdOrKeyName(IdOrKeyName? aplicacionRef, bool asNoTracking, CancellationToken ct);

    Task<MasterEntityDto?> GetDtoById(
               Guid id,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtoById(
               MasterId id,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtotByKeyName(
               string keyNameString,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtotByKeyName(
               KeyNameVO keyName,
               CancellationToken cancellationToken = default
           );
    Task<MasterEntityDto?> GetDtoFromIdOrKeyName(IdOrKeyName? aplicacionRef, CancellationToken ct);
}
