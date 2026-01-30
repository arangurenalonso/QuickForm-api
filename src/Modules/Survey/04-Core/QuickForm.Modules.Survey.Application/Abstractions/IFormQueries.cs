using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application.Forms.Queries;

public interface IFormQueries
{
    Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default);

    Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
}
