using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(CancellationToken cancellationToken = default);
}
