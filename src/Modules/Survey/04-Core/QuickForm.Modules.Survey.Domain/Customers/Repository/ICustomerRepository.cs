namespace QuickForm.Modules.Survey.Domain.Customers;
public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(Customer customer);
}
