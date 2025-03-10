namespace QuickForm.Modules.Survey.Domain;
public interface ICustomerRepository
{
    Task<Customer?> GetAsync(CustomerId id, CancellationToken cancellationToken = default);

    void Insert(Customer customer);
}
