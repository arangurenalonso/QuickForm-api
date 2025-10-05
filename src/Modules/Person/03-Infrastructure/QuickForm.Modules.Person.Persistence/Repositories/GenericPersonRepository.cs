using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Persistence;
public class GenericPersonRepository<TEntity, TEntityId>
    : RepositoryBase<TEntity, TEntityId>, IGenericPersonRepository<TEntity, TEntityId>
    where TEntity : BaseDomainEntity<TEntityId>
    where TEntityId : EntityId
{
    public GenericPersonRepository(PersonDbContext context) : base(context) { }
}
