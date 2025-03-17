using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;

internal sealed class GetUserByIdQueryHandler(IUserDapperRepository _userDapperRepository )
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<ResultT<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
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
