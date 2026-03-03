using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetFormSubmissionsQueryHandler(
    IFormRepository _formRepository,
    IFormQueries _formQueries
) : ICommandHandler<GetFormSubmissionsQuery, FormSubmissionsResult>
{
    public async Task<ResultT<FormSubmissionsResult>> Handle(
        GetFormSubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var form = await _formRepository.GetFormToCheckActionAsync(request.IdForm, cancellationToken);
        if (form is null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return error;
        }

        var columns = await _formQueries.GetFormColumnsByIdFormAsync(request.IdForm, cancellationToken);
        var row = await _formQueries.GetFormRowsByIdFormAsync(request.IdForm,0,50, cancellationToken);
        var result = new FormSubmissionsResult(columns, row);
        return result;
    }

}
