using MediatR;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class FormPublishCommandHandler(
    IFormRepository formRepository, 
    IFormQueries formQueries,
    IUnitOfWork _unitOfWork,
    ISender sender)
    : ICommandHandler<FormPublishCommand, ResultResponse>
{
    public async Task<ResultT<ResultResponse>> Handle(FormPublishCommand request, CancellationToken cancellationToken)
    {

        if (request.Sections.Any())
        {
            var result = await sender.Send(new SaveFormStructureCommand(request.IdForm, request.Sections), cancellationToken);
            if (result.IsFailure)
            {
                return ResultT<ResultResponse>.FailureT(result.ResultType, result.Errors);
            }
        }


        var form = await formRepository.GetAsync(request.IdForm, cancellationToken);

        if (form == null)
        {
            var error = ResultError.NullValue("FormId", $"Form with id '{request.IdForm}' not found.");
            return ResultT<ResultResponse>.FailureT(ResultType.NotFound, error);
        }

        var sectionQuestionStatus = await formQueries.GetSectionQuestionStatusAsync(request.IdForm, cancellationToken);

        if (!sectionQuestionStatus.HasSections)
        {
            return ResultError.EmptyValue("Sections", $"Form must have at least one section to be published.");
        }

        if(!sectionQuestionStatus.AllSectionsHaveQuestions)
        {
            var sectionWithoutQuestions = sectionQuestionStatus.Sections.Where(s => !s.HasQuestions).Select(x=>x.SectionName);
            var sectionsWithoutQuestions = string.Join(", ", sectionWithoutQuestions);
            return  ResultError.EmptyValue("Questions", $"The following sections must have at least one question to be published: {sectionsWithoutQuestions}.");
        }

        var resultUpdate = form.Publish(false);

        if (resultUpdate.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(ResultType.DomainValidation, resultUpdate.Errors);
        }

        var resultTransaction = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (resultTransaction.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(resultTransaction.ResultType, resultTransaction.Errors);
        }

        return ResultResponse.Success($"Form id '{request.IdForm}' published successfully.");
    }
}
