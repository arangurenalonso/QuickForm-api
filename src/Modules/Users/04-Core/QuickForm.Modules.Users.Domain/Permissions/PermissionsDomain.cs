using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsDomain : BaseDomainEntity<PermissionsId>
{
    public ResourcesId IdResources { get; set; }
    public PermissionsActionsId IdAction { get; set; }
    public ResourcesDomain Resources { get; private set; }
    public PermissionsActionsDomain Action { get; private set; }


    #region One-to-Many Relationship
    public ICollection<RolePermissionsDomain> RolePermissions { get; private set; } = [];
    #endregion

    public PermissionsDomain() { }

    private PermissionsDomain(
        PermissionsId id,
        ResourcesDomain resources,
        PermissionsActionsDomain action) : base(id)
    {
        IdResources = resources.Id;
        IdAction = action.Id;
    }
    public static ResultT<PermissionsDomain> Create(
            ResourcesDomain resources,
            PermissionsActionsDomain action
       )
    {
        var newPermission = new PermissionsDomain(PermissionsId.Create(), resources, action);

        return newPermission;
    }
}
