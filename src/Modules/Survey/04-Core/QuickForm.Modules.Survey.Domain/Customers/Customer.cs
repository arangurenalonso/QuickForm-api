using System.ComponentModel.DataAnnotations.Schema;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public sealed class Customer : BaseDomainEntity<CustomerId>
{

    public string Email { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<FormDomain> Forms { get; private set; } = [];
    private Customer()
    {
    }
    private Customer(CustomerId id, string email) : base(id)
    {
        Email = email;
    }


    public static Customer Create(Guid id, string email)
    {
        CustomerId customerId =new CustomerId(id);

        return new Customer(customerId, email);
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
