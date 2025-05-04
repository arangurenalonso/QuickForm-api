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

    public async Task<List<QuestionDomain>> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        return await _context.Question
                        .Where(u => u.IdForm == formId && u.IsActive)
                        .ToListAsync(cancellationToken);
    }

    public void Insert(FormDomain form)
    {
        _context.Form.Add(form);
    }
}
