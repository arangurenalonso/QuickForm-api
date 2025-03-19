using System;
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
        ResourcesId idResources,
        PermissionsActionsId idAction) : base(id)
    {
        IdResources = idResources;
        IdAction = idAction;
    }
    public static ResultT<PermissionsDomain> Create(
            PermissionsId permissionsId,
            ResourcesId idResources,
            PermissionsActionsId idAction
       )
    {
        var newPermission = new PermissionsDomain(permissionsId, idResources, idAction);

        return newPermission;
    }
    public static ResultT<PermissionsDomain> Create(ResourcesId idResources,
            PermissionsActionsId idAction)
        => Create(PermissionsId.Create(), idResources, idAction);

    public Result Update(
           ResourcesId idResources,
           PermissionsActionsId idAction
       )
    {
        IdResources = idResources;
        IdAction = idAction;
        return Result.Success();
    }
}
