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
        return await _context.Form.FirstOrDefaultAsync(u => u.Id == formId && u.IsActive, cancellationToken);
    }

    public async Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        var query = _context
                        .FormSection
                        .Include(x => x.Questions.Where(section => section.IsActive))
                            .ThenInclude(x => x.QuestionAttributeValue.Where(q => q.IsActive))
                        .AsSplitQuery()
                        .Where(u => u.IdForm == formId && u.IsActive);

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
                        .Include(x => x.QuestionAttributeValue.Where(qav => qav.IsActive))
                        .ThenInclude(x => x.QuestionTypeAttribute)
                        .ThenInclude(x => x.Attribute)
                        .Where(u => u.FormSection.IdForm == formId && u.IsActive)
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
        return await _context.FormSection
                        .Where(u => u.IdForm == formId && u.IsActive)
                        .ToListAsync(cancellationToken);
    }
}
