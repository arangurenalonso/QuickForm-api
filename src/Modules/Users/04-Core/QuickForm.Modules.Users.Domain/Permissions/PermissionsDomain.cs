using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsDomain : BaseDomainEntity<PermissionsId>
{
    public PermissionResourcesId IdResources { get; set; }
    public PermissionsActionsId IdAction { get; set; }
    public PermissionResourcesDomain Resources { get; private set; }
    public PermissionsActionsDomain Action { get; private set; }
    public PermissionsDomain() { }

    private PermissionsDomain(
        PermissionsId id,
        PermissionResourcesDomain resources,
        PermissionsActionsDomain action) : base(id)
    {
        IdResources = resources.Id;
        IdAction = action.Id;
    }
    public static ResultT<PermissionsDomain> Create(
            PermissionResourcesDomain resources,
            PermissionsActionsDomain action
       )
    {
        var newPermission = new PermissionsDomain(PermissionsId.Create(), resources, action);

        return newPermission;
    }
}
