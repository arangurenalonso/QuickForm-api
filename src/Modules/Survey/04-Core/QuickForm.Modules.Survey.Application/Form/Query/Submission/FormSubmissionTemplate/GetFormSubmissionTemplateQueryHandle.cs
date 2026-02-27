using System.Reflection;
using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetFormSubmissionTemplateQueryHandle(
        IFormQueries _formQuery,
        IMediator _mediator
    )
    : IQueryHandler<GetFormSubmissionTemplateQuery, GetFormSubmissionTemplateViewModel>
{
    public async Task<ResultT<GetFormSubmissionTemplateViewModel>> Handle(GetFormSubmissionTemplateQuery request, CancellationToken cancellationToken)
    {
        var form = await _formQuery.GetFormByIdAsync(request.IdForm, cancellationToken);
        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<GetFormSubmissionTemplateViewModel>.FailureT(ResultType.NotFound, error);
        }

        var sectionsResult = await _mediator.Send(new GetFormStructureQuery(
                form.Id
            ), cancellationToken);
        if (!sectionsResult.IsSuccess)
        {
            return ResultT<GetFormSubmissionTemplateViewModel>.FailureT(sectionsResult.ResultType, sectionsResult.Errors);
        }

        return new GetFormSubmissionTemplateViewModel(form, sectionsResult.Value);
    }

}
