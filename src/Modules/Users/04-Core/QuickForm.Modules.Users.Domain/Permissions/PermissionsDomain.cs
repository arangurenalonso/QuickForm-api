using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public class PermissionsDomain : BaseDomainEntity<PermissionsId>
{
    public KeyNameVO KeyName { get; private set; }
    public PathUrlVO PathUrl { get; private set; }
    public HttpMethodVO HttpMethod { get; private set; }
    public MasterId IdApplication { get; private set; }
    public MasterId IdResources { get; private set; }
    public MasterId IdAction { get; private set; }

    public ApplicationDomain Application { get; private set; } = null!;
    public ResourcesDomain Resources { get; private set; } = null!;
    public PermissionsActionsDomain Action { get; private set; } = null!;

    public ICollection<RolePermissionsDomain> RolePermissions { get; private set; } = [];

    private PermissionsDomain() { }

    private PermissionsDomain(
        PermissionsId id,
        KeyNameVO keyName,
        PathUrlVO pathUrl,
        HttpMethodVO httpMethod,
        MasterId idApplication,
        MasterId idResources,
        MasterId idAction) : base(id)
    {
        KeyName = keyName;
        PathUrl = pathUrl;
        HttpMethod = httpMethod;
        IdApplication = idApplication;
        IdResources = idResources;
        IdAction = idAction;
    }

    public static ResultT<PermissionsDomain> Create(
        string keyName,
        string pathUrl,
        string httpMethod,
        MasterId idApplication,
        MasterId idResources,
        MasterId idAction)
    {
        var keyNameResult = KeyNameVO.Create(keyName);
        var pathResult = PathUrlVO.Create(pathUrl);
        var methodResult = HttpMethodVO.Create(httpMethod);

        if (keyNameResult.IsFailure || pathResult.IsFailure || methodResult.IsFailure)
        {
            return new ResultErrorList(new List<ResultErrorList>
            {
                keyNameResult.Errors,
                pathResult.Errors,
                methodResult.Errors
            });
        }

        return new PermissionsDomain(
            PermissionsId.Create(),
            keyNameResult.Value,
            pathResult.Value,
            methodResult.Value,
            idApplication,
            idResources,
            idAction);
    }

    public Result Update(
        string keyName,
        string pathUrl,
        string httpMethod,
        MasterId idApplication,
        MasterId idResources,
        MasterId idAction)
    {
        var keyNameResult = KeyNameVO.Create(keyName);
        var pathResult = PathUrlVO.Create(pathUrl);
        var methodResult = HttpMethodVO.Create(httpMethod);

        if (keyNameResult.IsFailure || pathResult.IsFailure || methodResult.IsFailure)
        {
            return new ResultErrorList(new List<ResultErrorList>
            {
                keyNameResult.Errors,
                pathResult.Errors,
                methodResult.Errors
            });
        }

        KeyName = keyNameResult.Value;
        PathUrl = pathResult.Value;
        HttpMethod = methodResult.Value;
        IdApplication = idApplication;
        IdResources = idResources;
        IdAction = idAction;

        return Result.Success();
    }
}
