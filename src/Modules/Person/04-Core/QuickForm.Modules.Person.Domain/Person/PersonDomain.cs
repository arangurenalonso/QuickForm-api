using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
namespace QuickForm.Modules.Person.Domain;
public sealed class PersonDomain : BaseDomainEntity<PersonId>
{
    public NameVO Name { get; private set; }
    public LastNameVO? LastName { get; private set; }
    public PersonDomain() { }

    private PersonDomain(
            PersonId id,
            NameVO name,
            LastNameVO lastName
        ) : base(id)
    {
        Name = name;
        LastName = lastName;
    }

    public static ResultT<PersonDomain> Create(
            string firstName,
            string? lastName
        )
    {
        var nameResult = NameVO.Create(firstName);
        var lastNameResult = LastNameVO.Create(lastName);
        if (nameResult.IsFailure || lastNameResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() {  nameResult.Errors, lastNameResult.Errors }
                );
            return errorList;
        }
        var newDomain = new PersonDomain(
                                PersonId.Create(), 
                                nameResult.Value, 
                                lastNameResult.Value
                                );


        return newDomain;
    }

}
