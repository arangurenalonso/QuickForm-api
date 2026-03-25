using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain;
namespace QuickForm.Modules.Survey.Persistence;

public sealed class FormQueries(SurveyDbContext _context) : IFormQueries
{

    public async Task<FormSectionQuestionStatusResponse> GetSectionQuestionStatusAsync(
        Guid idForm,
        CancellationToken cancellationToken = default)
    {
        FormId formId = new(idForm);

        var sections = await _context.Set<FormSectionDomain>()
            .Where(section => section.IdForm == formId && !section.IsDeleted)
            .Select(section => new SectionQuestionStatusResponse
            {
                SectionId = section.Id.Value,
                SectionName = section.Title.Value,
                HasQuestions = section.Questions.Any(question => !question.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return new FormSectionQuestionStatusResponse
        {
            Sections = sections
        };
    }
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
            Updated = x.ModifiedDate == null
                                    ? x.CreatedDate
                                    : x.ModifiedDate.Value,
            Submissions = x.Submissions.Count(s => !s.IsDeleted),
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

    public async Task<List<OptionsViewModel>> GetFormStatusAsOptionsViewModel(CancellationToken cancellationToken = default)
    {
        return await _context.Set<FormStatusDomain>()
            .AsNoTracking()
                .Select(av => new OptionsViewModel
                {
                    Key = av.Id.Value.ToString(),
                    Value = av.KeyName.Value
                })
            .ToListAsync(cancellationToken);
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
                QuestionTypeId = q.IdQuestionType.Value,
                QuestionTypeKey = q.QuestionType.KeyName.Value
            })
            .ToListAsync(ct);
    }



    public async Task<ResultT<PaginationResult<FormViewModel>>> SearchFormAsync(
           Guid idCustomer,
           List<FiltersForm>? filters,
           int skip = 0,
           int take = 50,
           CancellationToken ct = default
        )
    {
        var customerId = new CustomerId(idCustomer);
        IQueryable<FormDomain> baseQuery = _context.Set<FormDomain>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.IdCustomer == customerId);

        var filterResult = ApplyFilters(baseQuery, filters);
        if (filterResult.IsFailure)
        {
            return ResultT<PaginationResult<FormViewModel>>.FailureT(ResultType.BadRequest, filterResult.Errors);
        }

        baseQuery = filterResult.Value;

        var totalCount = await baseQuery.CountAsync(ct);

        var normalizedSkip = skip < 0 ? 0 : skip;

        var pageSize = take <= 0 ? 10 : take;

        var currentPage = normalizedSkip / pageSize + 1;

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        if (totalCount == 0)
        {
            return new PaginationResult<FormViewModel>
            {
                Items = [],
                TotalCount = 0,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = 0,
            };
        }
        var rows = await baseQuery
                            .OrderByDescending(f => f.CreatedDate)
                            .Skip(normalizedSkip)
                            .Take(pageSize)
                            .Select(FormToViewModel())
                            .ToListAsync(ct);

        return new PaginationResult<FormViewModel>
        {
            Items = rows,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = totalPages
        };
    }

    private ResultT<IQueryable<FormDomain>> ApplyFilters(
        IQueryable<FormDomain> query,
        List<FiltersForm>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return ResultT<IQueryable<FormDomain>>.Success(query);
        }

        foreach (var filter in filters)
        {
            var filterResult = ApplySingleFilter(query, filter);
            if (filterResult.IsFailure)
            {
                return filterResult.Errors;
            }

            query = filterResult.Value;
        }

        return ResultT<IQueryable<FormDomain>>.Success(query);
    }

    private ResultT<IQueryable<FormDomain>> ApplySingleFilter(
        IQueryable<FormDomain> query,
        FiltersForm filter)
    {
        var operatorType = EnumExtensions.FromId<ConditionalOperatorType>(filter.OperatorId);
        if (operatorType is null)
        {
            return ResultT<IQueryable<FormDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidFormat(
                    "Filter.OperatorId",
                    $"Operator id '{filter.OperatorId}' is invalid."));
        }

        if (filter.ColumnKey.Equals("submittedAt", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(s => s.ModifiedDate.HasValue);
            return ApplyComparableFilter(
                query,
                filter,
                operatorType.Value,
                s => s.Status.CreatedDate,
                CommonJsonElementMethods.TryGetDateTime,
                "datetime");
        }
        return ResultT<IQueryable<FormDomain>>.Success(query);

    }



    private delegate bool TryParseFilterValue<T>(JsonElement element, out T value);
    private static ResultT<(JsonElement From, JsonElement To)> GetBetweenValues(FiltersForm filter)
    {
        if (CommonJsonElementMethods.IsNullOrUndefined(filter.Value))
        {
            return ResultT<(JsonElement From, JsonElement To)>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    "Value is required for operator 'Between'."));
        }

        if (CommonJsonElementMethods.IsNullOrUndefined(filter.SecondValue))
        {
            return ResultT<(JsonElement From, JsonElement To)>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.SecondValue",
                    "SecondValue is required for operator 'Between'."));
        }

        return ResultT<(JsonElement From, JsonElement To)>.Success(
            (filter.Value!.Value, filter.SecondValue!.Value));
    }
    private ResultT<IQueryable<TEntity>> ApplyComparableFilter<TEntity, T>(
            IQueryable<TEntity> query,
            FiltersForm filter,
            ConditionalOperatorType operatorType,
            Expression<Func<TEntity, T>> selector,
            TryParseFilterValue<T> tryParse,
            string fieldName
        )
        where T : struct, IComparable<T>
    {
        if (operatorType == ConditionalOperatorType.Between)
        {
            var betweenResult = GetBetweenValues(filter);
            if (betweenResult.IsFailure)
            {
                return ResultT<IQueryable<TEntity>>.FailureT(
                    ResultType.BadRequest,
                    betweenResult.Errors);
            }

            var (fromElement, toElement) = betweenResult.Value;

            if (!tryParse(fromElement, out var from))
            {
                return ResultT<IQueryable<TEntity>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.Value",
                        $"The value '{fromElement.GetRawText()}' is not a valid {fieldName}."));
            }

            if (!tryParse(toElement, out var to))
            {
                return ResultT<IQueryable<TEntity>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"The value '{toElement.GetRawText()}' is not a valid {fieldName}."));
            }

            if (from.CompareTo(to) > 0)
            {
                return ResultT<IQueryable<TEntity>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"SecondValue '{to}' must be greater than or equal to Value '{from}'."));
            }

            query = query.Where(BuildBetweenExpression(selector, from, to));

            return ResultT<IQueryable<TEntity>>.Success(query);
        }

        if (CommonJsonElementMethods.IsNullOrUndefined(filter.Value))
        {
            return ResultT<IQueryable<TEntity>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"Value is required for column '{filter.ColumnKey}' and operator '{operatorType}'."));
        }

        if (!tryParse(filter.Value!.Value, out var value))
        {
            return ResultT<IQueryable<TEntity>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The value '{filter.Value.Value.GetRawText()}' is not a valid {fieldName}."));
        }

        var predicate = BuildComparisonExpression(selector, operatorType, value);

        if (predicate is null)
        {
            return ResultT<IQueryable<TEntity>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for column '{filter.ColumnKey}'."));
        }

        query = query.Where(predicate);

        return ResultT<IQueryable<TEntity>>.Success(query);
    }
    private static Expression<Func<TEntity, bool>> BuildBetweenExpression<TEntity, T>(
        Expression<Func<TEntity, T>> selector,
        T from,
        T to
     ) where T : struct, IComparable<T>
    {
        var parameter = selector.Parameters[0];
        var left = selector.Body;
        var fromConstant = Expression.Constant(from, typeof(T));
        var toConstant = Expression.Constant(to, typeof(T));

        var greaterThanOrEqual = Expression.GreaterThanOrEqual(left, fromConstant);
        var lessThanOrEqual = Expression.LessThanOrEqual(left, toConstant);
        var body = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private static Expression<Func<TEntity, bool>>? BuildComparisonExpression<TEntity, T>(
        Expression<Func<TEntity, T>> selector,
        ConditionalOperatorType operatorType,
        T value)
        where T : struct, IComparable<T>
    {
        var parameter = selector.Parameters[0];
        var left = selector.Body;
        var right = Expression.Constant(value, typeof(T));

        Expression? body = operatorType switch
        {
            ConditionalOperatorType.On => Expression.Equal(left, right),
            ConditionalOperatorType.After => Expression.GreaterThan(left, right),
            ConditionalOperatorType.OnOrAfter => Expression.GreaterThanOrEqual(left, right),
            ConditionalOperatorType.Before => Expression.LessThan(left, right),
            ConditionalOperatorType.OnOrBefore => Expression.LessThanOrEqual(left, right),
            _ => null
        };

        return body is null
            ? null
            : Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
    private ResultT<IQueryable<SubmissionDomain>> ApplySubmittedAtFilter(
    IQueryable<SubmissionDomain> query,
    FiltersForm filter,
    ConditionalOperatorType operatorType)
    {
        return ApplyComparableFilter(
            query,
            filter,
            operatorType,
            s => s.SubmittedAtUtc,
            CommonJsonElementMethods.TryGetDateTime,
            "datetime");
    }
}
