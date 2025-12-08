using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public class CountryDomain : BaseMasterEntity
{
    private CountryDomain() { }

    private CountryDomain(MasterId id) : base(id) { }


    public static ResultT<CountryDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new CountryDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);

        var result = newDomain.SetBaseProperties(masterUpdateBase);

        if (result.IsFailure)
        {
            return result.Errors;
        }      

        return newDomain;
    }
    public static ResultT<CountryDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new CountryDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }      
        return newDomain;
    }

}
