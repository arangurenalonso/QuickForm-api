using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(string originClass, CancellationToken cancellationToken = default);
}
