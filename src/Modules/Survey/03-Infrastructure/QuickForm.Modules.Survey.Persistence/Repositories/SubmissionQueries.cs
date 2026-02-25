using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public sealed class SubmissionQueries(SurveyDbContext _context) : ISubmissionQueries
{
    public async Task<List<SubmissionExportRow>> GetSubmissionsForExportAsync(
        Guid idForm,
        CancellationToken ct = default)
    {
        var formId = new FormId(idForm);

        return await _context.Set<SubmissionDomain>()
            .AsNoTracking()
            .Where(s => s.IdForm == formId && !s.IsDeleted)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Select(s => new SubmissionExportRow(
                s.Id.Value,
                s.SubmittedAtUtc,
                s.SubmissionValues
                    .Where(v => !v.IsDeleted) 
                    .Select(v => new SubmissionValueExport(
                        v.IdQuestion.Value,
                        v.Value
                    ))
                    .ToList()
            ))
            .ToListAsync(ct);
    }
}
