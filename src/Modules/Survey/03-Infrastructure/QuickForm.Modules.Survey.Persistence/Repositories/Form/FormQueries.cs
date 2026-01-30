using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Application.Forms.Queries;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public sealed class FormQueries(SurveyDbContext _context) : IFormQueries
{
    public async Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default)
    {
        var customerId = new CustomerId(idCustomer);
        return await _context.Set<FormDomain>()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(u => u.IdCustomer == customerId && !u.IsDeleted)
            .Select(x => new FormViewModel
            {
                Id = x.Id.Value,
                Name = x.Name.Value,
                Description = x.Description,
                CreatedAt = x.CreatedDate,
                Status = new StatusViewModel
                {
                    Id = x.Status.Id.Value,
                    Name = x.Status.KeyName.Value,
                    Description = x.Status.Description != null ? x.Status.Description.Value : "",
                    Icon = x.Status.Icon != null ? x.Status.Icon.Value : "",
                    Color = x.Status.Color.Value,
                    AllowedActions = x.Status.Permissions
                        .Select(y => y.FormAction.KeyName.Value)
                        .ToList()
                }
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<FormSectionDomain>> GetStructureFormAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        FormId formId = new FormId(id);
        var query = _context
                        .Set<FormSectionDomain>()
                        .Include(x => x.Questions.Where(section => !section.IsDeleted))
                            .ThenInclude(x => x.QuestionRuleValue.Where(q => !q.IsDeleted))
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
}
