using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Person.Domain;
public sealed class PaisDomain : BaseDomainEntity<PaisId>
{
    public PaisDomain() { }

    private PaisDomain(
            PaisId id
        ) : base(id)
    {
    }

    public static ResultT<PaisDomain> Create(
        )
    {
        var newDomain = new PaisDomain(
                                PaisId.Create()
                                );


        return newDomain;
    }

}
