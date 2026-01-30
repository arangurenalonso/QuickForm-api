using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public class FormRepository(
        SurveyDbContext _context
    ) : IFormRepository
{

    public async Task<FormDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        return await _context.Set<FormDomain>()
                                .Include(x => x.Status)
                                .Include(x=>x.Sections.Where(y=>!y.IsDeleted))
                                    .ThenInclude(x=>x.Questions.Where(y => !y.IsDeleted))
                                        .ThenInclude(x=>x.QuestionAttributeValue.Where(y => !y.IsDeleted))
                                .Include(x => x.Sections.Where(y => !y.IsDeleted))
                                    .ThenInclude(x => x.Questions.Where(y => !y.IsDeleted))
                                        .ThenInclude(x => x.QuestionRuleValue.Where(y => !y.IsDeleted))
                                .AsSplitQuery()
                                .FirstOrDefaultAsync(u => u.Id == formId && !u.IsDeleted, cancellationToken);
    }

}
