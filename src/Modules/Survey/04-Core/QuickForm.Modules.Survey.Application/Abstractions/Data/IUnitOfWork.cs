using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;
public interface IUnitOfWork
{
    Task<ResultT<int>> SaveChangesWithResultAsync(string classOrigin, CancellationToken cancellationToken = default);
}
