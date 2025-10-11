using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class RegisterPermissionCommandHandler(
        IUnitOfWork _unitOfWork,
        IUserRepository _userRepository,
        IPasswordHashingService _passwordHashingService,
        IDateTimeProvider _dateTimeProvider,
        IRoleRepository _roleRepository
    ) : ICommandHandler<RegisterPermissionCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(RegisterPermissionCommand request, CancellationToken cancellationToken)
    {

        return ResultResponse.Success("User created successfully.");
    }

}

