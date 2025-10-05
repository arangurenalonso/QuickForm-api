using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class CustomerRepository(
    SurveyDbContext _context
    ) : ICustomerRepository
{
    public async Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        CustomerId customerId = new CustomerId(id);
        return await _context.Set<Customer>().SingleOrDefaultAsync(c => c.Id == customerId, cancellationToken);
    }


    public void Insert(Customer customer)
    {
        _context.Set<Customer>().Add(customer);
    }
}
