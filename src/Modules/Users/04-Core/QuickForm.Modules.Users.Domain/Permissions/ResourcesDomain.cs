using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class ResourcesDomain : BaseDomainEntity<ResourcesId>
{

    public PermissionResourcesDescription Description { get; private set; }

    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    public ResourcesDomain() { }

    private ResourcesDomain(
        ResourcesId id,
        PermissionResourcesDescription description) : base(id)
    {
        Description = description;
    }

    public static ResultT<ResourcesDomain> Create(
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
        var newPermissionResource = new ResourcesDomain(ResourcesId.Create(), descriptionResult.Value);

        return newPermissionResource;
    }

}
