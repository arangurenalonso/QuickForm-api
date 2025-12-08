using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsActionsDomain : BaseMasterEntity
{
    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];

    public PermissionsActionsDomain() { }
    private PermissionsActionsDomain(MasterId id) : base(id) { }

    public static ResultT<PermissionsActionsDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newId = MasterId.Create();
        return Create(newId, keyName, description);
    }
    public static ResultT<PermissionsActionsDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new PermissionsActionsDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }
        return newDomain;
    }

}
