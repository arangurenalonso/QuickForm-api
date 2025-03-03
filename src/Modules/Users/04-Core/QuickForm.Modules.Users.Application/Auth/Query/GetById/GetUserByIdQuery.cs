using QuickForm.Common.Application;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;

public sealed record GetUserByIdQuery(
    Guid Id
    ) : IQuery<UserDomain?>;
