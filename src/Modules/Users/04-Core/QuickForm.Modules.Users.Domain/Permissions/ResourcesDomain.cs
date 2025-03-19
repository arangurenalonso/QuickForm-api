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

    public static ResultT<ResourcesDomain> Create(string description)
        => Create(ResourcesId.Create(), description);

    public static ResultT<ResourcesDomain> Create(ResourcesId id, string description)
    {
        var descriptionResult = ValidateDescription(description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Errors;
        }

        return new ResourcesDomain(id, descriptionResult.Value);
    }

    private static ResultT<PermissionResourcesDescription> ValidateDescription(string description)
    {
        var descriptionResult = PermissionResourcesDescription.Create(description);
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
