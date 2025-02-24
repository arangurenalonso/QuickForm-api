using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record TokenValidationCommand(
    string Token
    ) : ICommand<ResultResponse>;
