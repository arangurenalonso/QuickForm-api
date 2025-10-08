using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsDomain : BaseDomainEntity<PermissionsId>
{
    public MasterId IdResources { get; set; }
    public MasterId IdAction { get; set; }
    public ResourcesDomain Resources { get; private set; }
    public PermissionsActionsDomain Action { get; private set; }


    #region One-to-Many Relationship
    public ICollection<RolePermissionsDomain> RolePermissions { get; private set; } = [];
    #endregion

    public PermissionsDomain() { }

    private PermissionsDomain(
        PermissionsId id,
        MasterId idResources,
        MasterId idAction) : base(id)
    {
        IdResources = idResources;
        IdAction = idAction;
    }
    public static ResultT<PermissionsDomain> Create(
            PermissionsId permissionsId,
            MasterId idResources,
            MasterId idAction
       )
    {
        var newPermission = new PermissionsDomain(permissionsId, idResources, idAction);

        return newPermission;
    }
    public static ResultT<PermissionsDomain> Create(
            MasterId idResources,
            MasterId idAction)
        => Create(PermissionsId.Create(), idResources, idAction);

    public Result Update(
           MasterId idResources,
           MasterId idAction
       )
    {
        IdResources = idResources;
        IdAction = idAction;
        return Result.Success();
    }
}
