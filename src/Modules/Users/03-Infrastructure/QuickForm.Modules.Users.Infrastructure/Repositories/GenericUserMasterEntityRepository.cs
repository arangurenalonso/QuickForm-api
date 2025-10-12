using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class GenericUserMasterEntityRepository<TEntity>
    : RepositoryMasterEntities<TEntity>, IGenericUserMasterEntityRepository<TEntity>
    where TEntity : BaseMasterEntity
{
    public GenericUserMasterEntityRepository(UsersDbContext context) : base(context) { }
}

