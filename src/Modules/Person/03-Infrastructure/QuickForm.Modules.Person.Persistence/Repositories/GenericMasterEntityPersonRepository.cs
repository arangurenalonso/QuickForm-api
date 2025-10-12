using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Persistence;
public class GenericMasterEntityPersonRepository<TEntity>
    : RepositoryMasterEntities<TEntity>, IGenericMasterEntityPersonRepository<TEntity>
    where TEntity : BaseMasterEntity
{
    public GenericMasterEntityPersonRepository(PersonDbContext context) : base(context) { }
}

