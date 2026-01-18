using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application.Forms.Queries;
namespace QuickForm.Modules.Survey.Application;
internal sealed class GetMyFormsQueryHandler(
        IFormQueries _formQuery,
        ICurrentUserService _currentUserService
    )
    : IQueryHandler<GetMyFormsQuery, List<FormViewModel>>
{
    public async Task<ResultT<List<FormViewModel>>> Handle(GetMyFormsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = _currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<List<FormViewModel>>.FailureT(ResultType.Unauthorized, userIdResult.Errors);
        }
        Guid userId = userIdResult.Value;
        var forms = await _formQuery.GetFormsByCustomerIdAsync(userId, cancellationToken);
        return forms;
    }

}
