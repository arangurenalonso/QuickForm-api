using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
