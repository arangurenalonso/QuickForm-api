using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record GetCurrentUserQuery() : IQuery<UserResponse>;
