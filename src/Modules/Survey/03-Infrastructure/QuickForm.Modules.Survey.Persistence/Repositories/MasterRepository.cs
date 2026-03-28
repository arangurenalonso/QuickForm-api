using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class MasterRepository(
    SurveyDbContext _context
    ) : IMasterRepository
{
    public async Task<List<TypeRenderFormViewModel>> GetTypesRenderQuery(CancellationToken ct = default)
    {
        return await _context.Set<FormRenderDomain>()
                                .Select(t => new TypeRenderFormViewModel
                                {
                                    Id = t.Id.Value,
                                    KeyName = t.KeyName.Value,
                                    Description = t.Description == null ? null : t.Description.Value,
                                    Icon = t.Icon == null ? null : t.Icon.Value,
                                    Color = t.Color == null ? null : t.Color.Value
                                })
                                .ToListAsync(ct);


    }
}

