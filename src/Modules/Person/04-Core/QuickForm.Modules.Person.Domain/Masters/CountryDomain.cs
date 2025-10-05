using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public class CountryDomain : BaseMasterEntity
{
    private CountryDomain() { }


    public static ResultT<CountryDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new CountryDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
    public Result Update(
            string keyName,
            string? description = null
        )
    {
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        return SetBaseProperties(masterUpdateBase);
    }

}
