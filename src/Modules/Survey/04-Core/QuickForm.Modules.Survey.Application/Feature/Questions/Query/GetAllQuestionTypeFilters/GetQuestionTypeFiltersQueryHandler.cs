using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;

internal sealed class GetAllQuestionTypeFiltersQueryHandler(
    IQuestionQueries _questionQueries
) : IQueryHandler<GetAllQuestionTypeFiltersQuery, IReadOnlyList<QuestionTypeFiltersGroupDto>>
{
    public async Task<ResultT<IReadOnlyList<QuestionTypeFiltersGroupDto>>> Handle(
        GetAllQuestionTypeFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _questionQueries.GetAllQuestionTypeFiltersAsync(cancellationToken);
        return result;
    }
}
