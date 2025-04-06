using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed class Customer : BaseDomainEntity<CustomerId>
{

    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public ICollection<FormDomain> Forms { get; private set; } = [];
    private Customer()
    {
    }
    private Customer(CustomerId id, string email, string firstName, string lastName) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }


    public static Customer Create(Guid id, string email, string firstName, string lastName)
    {
        CustomerId customerId =new CustomerId(id);

        return new Customer(customerId, email, firstName, lastName);
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
