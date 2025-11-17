using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class ResourcesDomain : BaseMasterEntity
{

    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    public ResourcesDomain() { }

    public static ResultT<ResourcesDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new ResourcesDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }


}
