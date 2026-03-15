using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record LogoutCommand(
    string RefreshToken
) : ICommand<ResultResponse>;
