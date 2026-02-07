using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Application.Forms.Queries;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;

public sealed class FormQueries(SurveyDbContext _context) : IFormQueries
{
    public async Task<bool> FormBelongsToCustomerAsync(
    Guid idForm,
    Guid idCustomer,
    CancellationToken ct = default)
    {
        var formId = new FormId(idForm);
        var customerId = new CustomerId(idCustomer);

        return await _context.Set<FormDomain>()
            .AsNoTracking()
            .AnyAsync(f =>
                f.Id == formId &&
                f.IdCustomer == customerId &&
                !f.IsDeleted,
                ct);
    }
    public async Task<FormViewModel?> GetFormByIdAsync(Guid idForm, CancellationToken ct = default)
    {
        var formId = new FormId(idForm);

        return await QueryForms()
            .Where(f => f.Id == formId)
            .Select(FormToViewModel())
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<FormViewModel>> GetFormsByCustomerIdAsync(Guid idCustomer, CancellationToken ct = default)
    {
        var customerId = new CustomerId(idCustomer);

        return await QueryForms()
            .Where(f => f.IdCustomer == customerId)
            .OrderByDescending(f => f.CreatedDate)
            .Select(FormToViewModel())
            .ToListAsync(ct);
    }

    private IQueryable<FormDomain> QueryForms()
    {
        return _context.Set<FormDomain>()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(f => !f.IsDeleted);
    }

    private Expression<Func<FormDomain, FormViewModel>> FormToViewModel()
    {
        return x => new FormViewModel
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
                    .Select(p => p.FormAction.KeyName.Value)
                    .ToList()
            }
        };
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
