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


    public static ResultT<PermissionsActionsDomain> Create(string description)
        => Create(PermissionsActionsId.Create(), description);

    public static ResultT<PermissionsActionsDomain> Create(
            PermissionsActionsId id,
            string description
        )
    {
        var descriptionResult = ValidateDescription(description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Errors;
        }
        var newRole = new PermissionsActionsDomain(id, descriptionResult.Value);

        return newRole;
    }
    private static ResultT<PermissionsActionsDescription> ValidateDescription(string description)
    {
        var descriptionResult = PermissionsActionsDescription.Create(description);
        return descriptionResult.IsFailure ? descriptionResult.Errors : descriptionResult;
    }

    public Result Update(
           string description
       )
    {
        var descriptionResult = ValidateDescription(description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Errors;
        }
        Description = descriptionResult.Value;
        return Result.Success();
    }


}
