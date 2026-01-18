namespace QuickForm.Modules.Survey.Application.Forms.Queries;

public interface IFormQueries
{
    Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default);
}
