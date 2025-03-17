using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;
public sealed class RoleDomain : BaseDomainEntity<RoleId>
{

    public RoleDescriptionVO Description { get; private set; }
    #region Many-to-Many Relationship
    public ICollection<UserRoleDomain> UserRole { get; private set; } = [];
    #endregion
    public RoleDomain() { }

    private RoleDomain(
        RoleId id,
        RoleDescriptionVO description) : base(id)
    {
        Description = description;
    }

    public static ResultT<RoleDomain> Create(
            string description
        )
    {
        var descriptionResult = RoleDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        var newRole = new RoleDomain(RoleId.Create(), descriptionResult.Value);

        return newRole;
    }
    public static ResultT<RoleDomain> Create(
            RoleId id,
            string description
        )
    {
        var descriptionResult = RoleDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        var newRole = new RoleDomain(id, descriptionResult.Value);

        return newRole;
    }
    public Result Update(
           string description
       )
    {
        var descriptionResult = RoleDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        Description = descriptionResult.Value;
        return Result.Success();
    }

}
