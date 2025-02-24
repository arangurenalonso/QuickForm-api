using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Persistence;

public class FormRepository(
    SurveyDbContext _context
    ) : IFormRepository
{
    public async Task<FormDomain?> GetAsync(FormId id, CancellationToken cancellationToken = default)
    {
        return await _context.Form.FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
    }

    public void Insert(FormDomain form)
    {
        _context.Form.Add(form);
    }
}
