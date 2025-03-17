using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class PermissionResourcesDomain : BaseDomainEntity<PermissionResourcesId>
{

    public PermissionResourcesDescription Description { get; private set; }

    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    public PermissionResourcesDomain() { }

    private PermissionResourcesDomain(
        PermissionResourcesId id,
        PermissionResourcesDescription description) : base(id)
    {
        Description = description;
    }

    public static ResultT<PermissionResourcesDomain> Create(
            string description
        )
    {
        var descriptionResult = PermissionResourcesDescription.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        var newPermissionResource = new PermissionResourcesDomain(PermissionResourcesId.Create(), descriptionResult.Value);

        return newPermissionResource;
    }

}
