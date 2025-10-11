using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;
public sealed record RegisterPermissionCommand(
        List<EndpointInfo> Endpoints
    ) : ICommand<ResultResponse>;

public sealed record EndpointInfo(
    string Pattern,
    string[] Methods,
    string? Name,
    string? DisplayName,
    string[] Tags,
    bool RequiresAuthorization
);
