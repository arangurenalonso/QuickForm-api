using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Application;

internal sealed class GetCurrentUserQueryHandler(
    IUserDapperRepository _userDapperRepository,
    ICurrentUserService _currentUserService
) : IQueryHandler<GetCurrentUserQuery, UserResponse>
{
    public async Task<ResultT<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserIdResult = _currentUserService.UserId;
        if (currentUserIdResult.IsFailure)
        {
            return ResultT<UserResponse>.FailureT(currentUserIdResult.ResultType, currentUserIdResult.Errors);
        }

        UserResponse? user = await _userDapperRepository.GetUserById(currentUserIdResult.Value);

        if (user is null)
        {
            var error = ResultError.NullValue("User", $"User with Id '{currentUserIdResult.Value}' Not Found");
            return ResultT<UserResponse>.FailureT(ResultType.NotFound, error);
        }

        return user;
    }
}
