using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application.Auth.Query.GetById;
public class GetUserByIdQueryHandler
{

}
public class ChangePasswordCommandHandler(
        IUserRepository _userRepository

    ) : IQueryHandler<GetUserByIdQuery, UserDomain?>
{

    public async Task<ResultT<UserDomain?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.Id);
        var user= await _userRepository.GetByIdAsync(userId);
        return ResultT<UserDomain?>.Success(user);
    }

}
