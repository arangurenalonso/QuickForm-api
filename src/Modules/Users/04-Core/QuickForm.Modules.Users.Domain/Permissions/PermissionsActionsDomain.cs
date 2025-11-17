using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsActionsDomain : BaseMasterEntity
{
    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];

    public PermissionsActionsDomain() { }

    public static ResultT<PermissionsActionsDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new PermissionsActionsDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }


}
