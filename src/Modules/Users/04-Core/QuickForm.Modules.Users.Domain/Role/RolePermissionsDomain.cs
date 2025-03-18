using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class RolePermissionsDomain : BaseDomainEntity<RolePermissionsId>
{
    public PermissionsId IdPermission { get; private set; }
    public RoleId IdRole { get; private set; }


    #region One to Many
    public PermissionsDomain Permission { get; private set; }
    public RoleDomain Role { get; private set; }
    #endregion


    private RolePermissionsDomain() { }

    private RolePermissionsDomain(
        RolePermissionsId id, PermissionsId idPermission, RoleId idRole
        ) : base(id)
    {
        IdPermission = idPermission;
        IdRole = idRole;
    }

    public static ResultT<RolePermissionsDomain> Create(PermissionsId idPermission, RoleId idRole)
    {
        return new RolePermissionsDomain(
            RolePermissionsId.Create(),
            idPermission,
            idRole
            );
    }

}
