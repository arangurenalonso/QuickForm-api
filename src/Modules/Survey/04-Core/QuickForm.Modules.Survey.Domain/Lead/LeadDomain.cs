using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class LeadDomain : BaseDomainEntity<LeadId>
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    private LeadDomain() { }
    private LeadDomain(
        LeadId id,
        string name,
        string email,
        string phoneNumber) : base(id)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static ResultT<LeadDomain> Create(
        string name,
        string email,
        string phoneNumber)
    {
        return new LeadDomain(
            LeadId.Create(),
            name,
            email,
            phoneNumber);
    }



}

