using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class UserRoleDomain : BaseDomainEntity<UserRoleId>
{
    public UserId IdUser { get; private set; }
    public RoleId IdRole { get; private set; }


    #region One to Many
    public UserDomain User { get; private set; }
    public RoleDomain Role { get; private set; }
    #endregion


    private UserRoleDomain() { }

    private UserRoleDomain(
        UserRoleId id, UserId idUser, RoleId idRole
        ) : base(id)
    {
        IdUser = idUser;
        IdRole = idRole;
    }

    public static ResultT<UserRoleDomain> Create(UserId idUser, RoleId idRole)
    {
        return new UserRoleDomain(
            UserRoleId.Create(),
            idUser,
            idRole
            );
    }


}
