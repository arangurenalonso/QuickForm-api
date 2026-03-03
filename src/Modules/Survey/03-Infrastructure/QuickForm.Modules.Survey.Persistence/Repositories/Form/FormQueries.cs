using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Application;
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

    public async Task<List<QuestionForSubmission>> GetQuestionsForSubmissionAsync(
        Guid idForm,
        CancellationToken ct = default)
    {
        var formId = new FormId(idForm);

        return await _context.Set<QuestionDomain>()
            .AsNoTracking()
            .Where(q => !q.IsDeleted && !q.FormSection.IsDeleted && q.FormSection.IdForm == formId)
            .Select(q => new QuestionForSubmission(
                q.Id,
                q.IdQuestionType.Value,
                q.QuestionAttributeValue
                    .Where(av => !av.IsDeleted)
                    .Select(av => new ValueTuple<Guid, string?>(av.QuestionTypeAttribute.Attribute.Id.Value, av.Value))
                    .ToList(),
                q.QuestionRuleValue
                    .Where(rv => !rv.IsDeleted)
                    .Select(rv => new ValueTuple<Guid, string?, string, string?>(
                                                rv.QuestionTypeRule.Rule.Id.Value,
                                                rv.Value, 
                                                rv.Message?? rv.QuestionTypeRule.Rule.DefaultValidationMessageTemplate.ValidationMessage, 
                                                rv.QuestionTypeRule.Rule.DefaultValidationMessageTemplate.PlaceholderKey
                                            )
                    )
                    .ToList()
            ))
            .ToListAsync(ct);
    }

    public async Task<List<ColumnDto>> GetFormColumnsByIdFormAsync(Guid idForm, CancellationToken ct = default)
    {
        var formId = new FormId(idForm);

        var idLabelAttribute = new MasterId(AttributeType.Label.GetId());
        return await _context.Set<QuestionDomain>()
            .AsNoTracking()
            .Where(q => !q.IsDeleted && !q.FormSection.IsDeleted && q.FormSection.IdForm == formId)
            .OrderBy(q => q.FormSection.SortOrder)
            .ThenBy(q => q.SortOrder)
            .Select(q => new ColumnDto
            {
                Key = "q_" + q.Id.Value.ToString(),
                Label = q.QuestionAttributeValue
                    .Where(av => !av.IsDeleted && av.QuestionTypeAttribute.Attribute.Id == idLabelAttribute)
                    .Select(av => av.Value)
                    .FirstOrDefault() ?? "Not specify",
                Order = q.FormSection.SortOrder * 1000 + q.SortOrder,
                Type = q.QuestionType.KeyName.Value
            })
            .ToListAsync(ct);
    }
    public async Task<List<RowDto>> GetFormRowsByIdFormAsync(
        Guid idForm,
        int skip = 0,
        int take = 50,
        CancellationToken ct = default)
    {
        var formId = new FormId(idForm);

        var submissions = await _context.Set<SubmissionDomain>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.IdForm == formId)
            .OrderByDescending(s => s.CreatedDate)
            .Skip(skip)
            .Take(take)
            .Select(s => new
            {
                s.Id,
                SubmittedAt = s.CreatedDate
            })
            .ToListAsync(ct);

        if (submissions.Count == 0)
        {
            return [];
        }

        var submissionIds = submissions.Select(x => x.Id).ToList();

        var answers = await _context.Set<SubmissionValueDomain>()
            .Where(a => !a.IsDeleted && submissionIds.Contains(a.IdSubmission))
            .Select(a => new
            {
                SubmissionId = a.IdSubmission.Value,
                QuestionId = a.IdQuestion.Value,
                a.Value, 
                TypeKey = a.Question.QuestionType.KeyName.Value
            })
            .ToListAsync(ct);

        var answersBySubmission = answers
            .GroupBy(x => x.SubmissionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = new List<RowDto>(submissions.Count);

        foreach (var s in submissions)
        {
            var row = new RowDto
            {
                Id = s.Id.Value,
                SubmittedAt = s.SubmittedAt
            };

            if (answersBySubmission.TryGetValue(s.Id.Value, out var subAnswers))
            {
                foreach (var a in subAnswers)
                {
                    var key = "q_" + a.QuestionId;
                    var valueConvertedResult = SurveyCommonMethods.ConvertStoredStringValue(a.TypeKey, a.Value.Trim() );

                    row.Cells[key] = valueConvertedResult;
                }
            }

            rows.Add(row);
        }

        return rows;
    }

}
