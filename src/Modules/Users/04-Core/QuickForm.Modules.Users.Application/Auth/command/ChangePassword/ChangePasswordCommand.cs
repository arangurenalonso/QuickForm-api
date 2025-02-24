using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword
    ) : ICommand<ResultResponse>;
