﻿using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public class QuestionTypeRepository(
    SurveyDbContext _context
    ) : IQuestionTypeRepository
{
    public async Task<List<QuestionTypeDomain>> GetByTypeKeysAsync(List<string> keysName, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        List<QuestionTypeKeyNameVO> KeyNameVO = keysName.Select(x => QuestionTypeKeyNameVO.Create(x).Value).ToList();
        var query = _context.QuestionType
            .Include(x=>x.QuestionTypeAttributes)
            .ThenInclude(x => x.Attribute)
            .ThenInclude(x => x.DataType)
            .Where(x => x.IsActive && KeyNameVO.Contains(x.KeyName));
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        var result = await query.ToListAsync(cancellationToken);
        return result;

    }

    public async Task<List<QuestionTypeDomain>> GetByIdsAsync(List<QuestionTypeId> questionTypeIds, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _context.QuestionType
            .Include(x => x.QuestionTypeAttributes)
            .ThenInclude(x => x.Attribute)
            .ThenInclude(x => x.DataType)
            .Where(x => x.IsActive && questionTypeIds.Contains(x.Id));
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        var result = await query.ToListAsync(cancellationToken);
        return result;

    }
}
