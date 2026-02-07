using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application.Forms.Queries;

public interface IFormQueries
{
    Task<bool> FormBelongsToCustomerAsync(
    Guid idForm,
    Guid idCustomer,
    CancellationToken ct = default);
    Task<FormViewModel?> GetFormByIdAsync(Guid idForm, CancellationToken ct = default);
    Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default);

    Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
}
