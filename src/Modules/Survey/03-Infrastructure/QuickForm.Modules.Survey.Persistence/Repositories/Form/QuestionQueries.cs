using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public sealed class QuestionQueries(
    SurveyDbContext _context
) : IQuestionQueries
{
    public async Task<List<QuestionTypeFiltersGroupDto>> GetAllQuestionTypeFiltersAsync(
        CancellationToken ct = default)
    {
        var rows = await _context.Set<QuestionTypeFilterDomain>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new
            {
                QuestionTypeId = x.IdQuestionType.Value,
                QuestionTypeKey = x.QuestionType.KeyName.Value,
                QuestionTypeLabel = x.QuestionType.Description.Value,

                OperatorId = x.IdConditionalOperator.Value,
                OperatorKey = x.ConditionalOperator.KeyName.Value,
                OperatorLabel = x.ConditionalOperator.Description != null
                    ? x.ConditionalOperator.Description.Value
                    : x.ConditionalOperator.KeyName.Value,
                OperatorOrder = x.ConditionalOperator.SortOrder,

                UiControlTypeKey = x.UiControlType.KeyName.Value,
                UiControlTypeLabel = x.UiControlType.Description != null
                    ? x.UiControlType.Description.Value
                    : x.UiControlType.KeyName.Value
            })
            .ToListAsync(ct);

        var result = rows
            .GroupBy(x => new
            {
                x.QuestionTypeId,
                x.QuestionTypeKey,
                x.QuestionTypeLabel
            })
            .Select(group => new QuestionTypeFiltersGroupDto(
                group.Key.QuestionTypeId,
                group.Key.QuestionTypeKey,
                group.Key.QuestionTypeLabel,
                group
                    .OrderBy(item => item.OperatorOrder)
                    .Select(item => new QuestionTypeFilterOptionDto(
                        item.OperatorId,
                        item.OperatorKey,
                        item.OperatorLabel,
                        item.UiControlTypeKey,
                        item.UiControlTypeLabel
                    ))
                    .ToList()
            ))
            .OrderBy(x => x.QuestionTypeKey)
            .ToList();

        return result;
    }
}
