using System.Text.Json;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetMeFormsQueryHandler(
        ICurrentUserService _currentUserService,
        IQuestionTypeRepository _questionTypeRepository,
        IFormRepository _formRepository,
        IUnitOfWork _unitOfWork
    )
    : IQueryHandler<GetMeFormsQuery, List<FormStructureSectionReponse>>
{
    public async Task<ResultT<List<FormStructureSectionReponse>>> Handle(GetMeFormsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = _currentUserService.UserId;
        if (userIdResult.IsFailure)
        {
            return ResultT<List<FormStructureSectionReponse>>.FailureT(ResultType.NotFound, userIdResult.Errors);
        }
        var forms = await _formRepository.GetFormsByCustomerIdAsync(userIdResult.Value, cancellationToken);


    }

}
