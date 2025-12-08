using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class ResourcesDomain : BaseMasterEntity
{

    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    public ResourcesDomain() { }
    private ResourcesDomain(MasterId id) : base(id) { }

    public static ResultT<ResourcesDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newId = MasterId.Create();
        return Create(newId, keyName, description);
    }
    public static ResultT<ResourcesDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new ResourcesDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }
        return newDomain;
    }


}
