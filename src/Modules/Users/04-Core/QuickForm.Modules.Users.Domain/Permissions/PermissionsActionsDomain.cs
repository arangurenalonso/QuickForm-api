using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionsActionsDomain : BaseDomainEntity<PermissionsActionsId>
{

    public PermissionsActionsDescription Description { get; private set; }
    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    public PermissionsActionsDomain() { }

    private PermissionsActionsDomain(
        PermissionsActionsId id,
        PermissionsActionsDescription description) : base(id)
    {
        Description = description;
    }

    public static ResultT<PermissionsActionsDomain> Create(
            string description
        )
    {
        var descriptionResult = PermissionsActionsDescription.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        var newRole = new PermissionsActionsDomain(PermissionsActionsId.Create(), descriptionResult.Value);

        return newRole;
    }

}
