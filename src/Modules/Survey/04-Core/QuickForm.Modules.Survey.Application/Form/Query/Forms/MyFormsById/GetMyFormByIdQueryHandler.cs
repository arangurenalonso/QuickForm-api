using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application.Forms.Queries;
namespace QuickForm.Modules.Survey.Application;
internal sealed class GetMyFormByIdQueryHandler(
        IFormQueries _formQuery,
        ICurrentUserService _currentUserService
    )
    : IQueryHandler<GetMyFormByIdQuery, FormViewModel>
{
    public async Task<ResultT<FormViewModel>> Handle(GetMyFormByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = _currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<FormViewModel>.FailureT(ResultType.Unauthorized, userIdResult.Errors);
        }
        var form = await _formQuery.GetFormByIdAsync(request.IdForm, cancellationToken);
        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<FormViewModel>.FailureT(ResultType.NotFound, error);
        }

        return form;
    }

}
