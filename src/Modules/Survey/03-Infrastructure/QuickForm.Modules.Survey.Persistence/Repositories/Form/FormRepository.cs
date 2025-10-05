using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public class FormRepository(
        SurveyDbContext _context
    ) : IFormRepository
{
    public async Task<FormDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        return await _context.Set<FormDomain>().FirstOrDefaultAsync(u => u.Id == formId && !u.IsDeleted, cancellationToken);
    }

    public async Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        var query = _context
                        .Set<FormSectionDomain>()
                        .Include(x => x.Questions.Where(section => !section.IsDeleted))
                            .ThenInclude(x => x.QuestionAttributeValue.Where(q => !q.IsDeleted))
                        .AsSplitQuery()
                        .Where(u => u.IdForm == formId && !u.IsDeleted);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.ToListAsync(cancellationToken);
    }
    public async Task<List<QuestionDomain>> GetQuestionByIdFormAsync(Guid id,bool asNoTracking, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        var query = _context.Question
                        .Include(x => x.QuestionAttributeValue.Where(qav => !qav.IsDeleted))
                        .ThenInclude(x => x.QuestionTypeAttribute)
                        .ThenInclude(x => x.Attribute)
                        .Where(u => u.FormSection.IdForm == formId && !u.IsDeleted)
                        .AsSplitQuery();
        if (asNoTracking)
        {
            query=query.AsNoTracking();
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<FormSectionDomain>> GetSectionsByIdFormAsync(Guid id, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        return await _context.Set<FormSectionDomain>()
                        .Where(u => u.IdForm == formId && !u.IsDeleted)
                        .ToListAsync(cancellationToken);
    }
}
