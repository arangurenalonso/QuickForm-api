using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : ICommand<AuthSessionResponse>;
