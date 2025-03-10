using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class CustomerRepository(
    SurveyDbContext _context
    ) : ICustomerRepository
{
    public async Task<Customer?> GetAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }


    public void Insert(Customer customer)
    {
        _context.Customers.Add(customer);
    }
}
