using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;

internal sealed class GetUserQueryHandler(IUserDapperRepository _userDapperRepository )
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<ResultT<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userDapperRepository.GetUserById(request.UserId);

        if (user is null)
        {
            var error = ResultError.NullValue("User", $"User with Id '{request.UserId}' Not Found");
            return ResultT<UserResponse>.FailureT(ResultType.NotFound, error);
        }

        return user;
    }
}
