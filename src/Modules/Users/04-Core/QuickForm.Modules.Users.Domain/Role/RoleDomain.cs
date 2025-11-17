using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;
public sealed class RoleDomain : BaseMasterEntity
{

    #region One-to-Many Relationship
    public ICollection<UserRoleDomain> UserRole { get; private set; } = [];
    public ICollection<RolePermissionsDomain> RolePermissions { get; private set; } = [];
    #endregion
    public RoleDomain() { }
    private RoleDomain(MasterId id) : base(id) { }

    public static ResultT<RoleDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new RoleDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
    public static ResultT<RoleDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new RoleDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
}
