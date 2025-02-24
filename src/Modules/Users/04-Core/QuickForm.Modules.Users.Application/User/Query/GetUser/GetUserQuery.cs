using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
