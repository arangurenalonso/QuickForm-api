using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Common.Domain.Method;
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
                        v.ValueRaw
                    ))
                    .ToList()
            ))
            .ToListAsync(ct);
    }

    public async Task<ResultT<PaginationResult<RowDto>>> SearchSubmissionsByIdFormAsync(
           Guid idForm,
           List<FiltersForm>? filters,
           int skip = 0,
           int take = 50,
           CancellationToken ct = default
        )
    {
        var formId = new FormId(idForm);

        IQueryable<SubmissionDomain> baseQuery = _context.Set<SubmissionDomain>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.IdForm == formId);

        var filterResult = ApplyFilters(baseQuery, filters);
        if (filterResult.IsFailure)
        {
            return ResultT<PaginationResult<RowDto>>.FailureT(ResultType.BadRequest, filterResult.Errors);
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
            return new PaginationResult<RowDto>
            {
                Items = [],
                TotalCount = 0,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = 0,
            };
        }

        var submissions = await baseQuery
            .OrderByDescending(s => s.SubmittedAtUtc)
            .Skip(skip)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                SubmittedAt = s.SubmittedAtUtc
            })
            .ToListAsync(ct);

        if (submissions.Count == 0)
        {
            return new PaginationResult<RowDto>
            {
                Items = [],
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = totalPages
            };
        }

        var submissionIds = submissions.Select(x => x.Id).ToList();

        var answers = await _context.Set<SubmissionValueDomain>()
            .AsNoTracking()
            .Where(a => !a.IsDeleted && submissionIds.Contains(a.IdSubmission))
            .Select(a => new
            {
                SubmissionId = a.IdSubmission.Value,
                QuestionId = a.IdQuestion.Value,
                a.DisplayValue
            })
            .ToListAsync(ct);

        var answersBySubmission = answers
            .GroupBy(x => x.SubmissionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = new List<RowDto>(submissions.Count);

        foreach (var submission in submissions)
        {
            var row = new RowDto
            {
                Id = submission.Id.Value,
                SubmittedAt = submission.SubmittedAt
            };

            if (answersBySubmission.TryGetValue(submission.Id.Value, out var submissionAnswers))
            {
                foreach (var answer in submissionAnswers)
                {
                    row.Cells["q_" + answer.QuestionId] = answer.DisplayValue;
                }
            }

            rows.Add(row);
        }

        return new PaginationResult<RowDto>
        {
            Items = rows,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = totalPages
        };
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyFilters(
        IQueryable<SubmissionDomain> query,
        List<FiltersForm>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return ResultT<IQueryable<SubmissionDomain>>.Success(query);
        }

        foreach (var filter in filters)
        {
            var filterResult = ApplySingleFilter(query, filter);
            if (filterResult.IsFailure)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, filterResult.Errors);
            }

            query = filterResult.Value;
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplySingleFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter)
    {
        var operatorType = EnumExtensions.FromId<ConditionalOperatorType>(filter.OperatorId);
        if (operatorType is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidFormat(
                    "Filter.OperatorId",
                    $"Operator id '{filter.OperatorId}' is invalid."));
        }

        if (IsSubmissionField(filter.ColumnKey))
        {
            return ApplySubmissionFieldFilter(query, filter, operatorType.Value);
        }

        var questionIdResult = TryGetQuestionId(filter.ColumnKey);
        if (questionIdResult.IsFailure)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, questionIdResult.Errors);
        }

        var questionType = EnumExtensions.FromId<QuestionTypeType>(filter.QuestionTypeId);
        if (questionType is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidFormat(
                    "Filter.QuestionTypeId",
                    $"Question type id '{filter.QuestionTypeId}' is invalid."));
        }

        if (CommonJsonElementMethods.IsNullOrUndefined(filter.Value))
        {
            return ApplyNullValueOperator(query, questionIdResult.Value, questionType.Value, operatorType.Value);
        }

        return ApplyTypedQuestionFilter(query, filter, questionIdResult.Value, questionType.Value, operatorType.Value);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplySubmissionFieldFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        ConditionalOperatorType operatorType)
    {
        return filter.ColumnKey switch
        {
            "submittedAt" => ApplySubmittedAtFilter(query, filter, operatorType),

            // future extension point:
            // "submissionId" => ApplySubmissionIdFilter(query, filter, operatorType),
            // "status" => ApplyStatusFilter(query, filter, operatorType),
            // "createdByEmail" => ApplyCreatedByEmailFilter(query, filter, operatorType),

            _ => ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.ColumnKey",
                    $"Submission field '{filter.ColumnKey}' is not supported."))
        };
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplySubmittedAtFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        ConditionalOperatorType operatorType)
    {
        if (operatorType == ConditionalOperatorType.Between)
        {
            var betweenResult = GetBetweenValues(filter);
            if (betweenResult.IsFailure)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, betweenResult.Errors);
            }

            var (fromElement, toElement) = betweenResult.Value;

            if (!CommonJsonElementMethods.TryGetDateTime(fromElement, out var from))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.Value",
                        $"The value '{fromElement.GetRawText()}' is not a valid datetime."));
            }

            if (!CommonJsonElementMethods.TryGetDateTime(toElement, out var to))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"The value '{toElement.GetRawText()}' is not a valid datetime."));
            }

            if (from > to)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"SecondValue {from} must be greater than or equal to Value {to}."));
            }

            query = query.Where(s => s.SubmittedAtUtc >= from && s.SubmittedAtUtc <= to);

            return ResultT<IQueryable<SubmissionDomain>>.Success(query);
        }
        if (CommonJsonElementMethods.IsNullOrUndefined(filter.Value))
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"Value is required for column '{filter.ColumnKey}' and operator '{operatorType}'."));
        }

        if (!CommonJsonElementMethods.TryGetDateTime(filter.Value!.Value, out var dateTimeValue))
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The value '{filter.Value.Value.GetRawText()}' is not a valid datetime."));
        }

        query = operatorType switch
        {
            ConditionalOperatorType.On => query.Where(s => s.SubmittedAtUtc == dateTimeValue),
            ConditionalOperatorType.After => query.Where(s => s.SubmittedAtUtc > dateTimeValue),
            ConditionalOperatorType.OnOrAfter => query.Where(s => s.SubmittedAtUtc >= dateTimeValue),
            ConditionalOperatorType.Before => query.Where(s => s.SubmittedAtUtc < dateTimeValue),
            ConditionalOperatorType.OnOrBefore => query.Where(s => s.SubmittedAtUtc <= dateTimeValue),
            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for column '{filter.ColumnKey}'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyNullValueOperator(
        IQueryable<SubmissionDomain> query,
        Guid questionIdGuid,
        QuestionTypeType questionType,
        ConditionalOperatorType operatorType)
    {
        var questionId = new QuestionId(questionIdGuid);
        switch (operatorType)
        {
            case ConditionalOperatorType.IsTrue:
                query = query.Where(s => s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueBoolean == true));
                return ResultT<IQueryable<SubmissionDomain>>.Success(query);
            case ConditionalOperatorType.IsFalse:
                query = query.Where(s => s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueBoolean == false));
                return ResultT<IQueryable<SubmissionDomain>>.Success(query);
            case ConditionalOperatorType.IsEmpty:
                return ApplyIsEmptyFilter(query, questionId, questionType);
            case ConditionalOperatorType.IsNotEmpty:
                return ApplyIsNotEmptyFilter(query, questionId, questionType);
            default:
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"Value is required for operator '{operatorType}' and question type '{questionType}'."));
        }
    }


    private ResultT<IQueryable<SubmissionDomain>> ApplyIsEmptyFilter(
        IQueryable<SubmissionDomain> query,
        QuestionId questionId,
        QuestionTypeType questionType)
    {
        query = questionType switch
        {
            QuestionTypeType.InputTypeText => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue))),

            QuestionTypeType.InputTypeInteger => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueInteger != null)),

            QuestionTypeType.InputTypeDecimal => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueDecimal != null)),

            QuestionTypeType.InputTypeBoolean => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueBoolean != null)),

            QuestionTypeType.InputTypeDatetime => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueDateTime != null)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The operator 'IsEmpty' is not supported for question type '{questionType}'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyIsNotEmptyFilter(
        IQueryable<SubmissionDomain> query,
        QuestionId questionId,
        QuestionTypeType questionType)
    {
        query = questionType switch
        {
            QuestionTypeType.InputTypeText => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue))),

            QuestionTypeType.InputTypeInteger => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueInteger != null)),

            QuestionTypeType.InputTypeDecimal => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueDecimal != null)),

            QuestionTypeType.InputTypeBoolean => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueBoolean != null)),

            QuestionTypeType.InputTypeDatetime => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    a.ValueDateTime != null)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The operator 'IsNotEmpty' is not supported for question type '{questionType}'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyTypedQuestionFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        Guid questionIdGuid,
        QuestionTypeType questionType,
        ConditionalOperatorType operatorType)
    {
        return questionType switch
        {
            QuestionTypeType.InputTypeText =>
                ApplyTextFilter(query, filter, questionIdGuid, operatorType),

            QuestionTypeType.InputTypeInteger =>
                ApplyIntegerFilter(query, filter, questionIdGuid, operatorType),

            QuestionTypeType.InputTypeDecimal =>
                ApplyDecimalFilter(query, filter, questionIdGuid, operatorType),

            QuestionTypeType.InputTypeDatetime =>
                ApplyDateTimeFilter(query, filter, questionIdGuid, operatorType),

            _ => ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "QuestionTypeId",
                    $"Filtering is not supported for question type '{questionType}'."))
        };
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyTextFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        Guid questionIdGuid,
        ConditionalOperatorType operatorType)
    {
        var questionId = new QuestionId(questionIdGuid);
        var stringValue = filter.Value!.Value.GetString()?.Trim() ?? string.Empty;
        var escapedLikeValue = EscapeLikeValue(stringValue);

        query = operatorType switch
        {
            ConditionalOperatorType.Contains => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    EF.Functions.Like(a.DisplayValue, $"%{escapedLikeValue}%"))),

            ConditionalOperatorType.NotContains => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    EF.Functions.Like(a.DisplayValue, $"%{escapedLikeValue}%"))),

            ConditionalOperatorType.StartsWith => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    EF.Functions.Like(a.DisplayValue, $"{escapedLikeValue}%"))),

            ConditionalOperatorType.EndsWith => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    EF.Functions.Like(a.DisplayValue, $"%{escapedLikeValue}"))),

            ConditionalOperatorType.Equals => query.Where(s =>
                s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    a.DisplayValue == stringValue)),

            ConditionalOperatorType.NotEquals => query.Where(s =>
                !s.SubmissionValues.Any(a =>
                    !a.IsDeleted &&
                    a.IdQuestion == questionId &&
                    !string.IsNullOrEmpty(a.DisplayValue) &&
                    a.DisplayValue == stringValue)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for question type 'InputTypeText'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyIntegerFilter(
         IQueryable<SubmissionDomain> query,
         FiltersForm filter,
         Guid questionIdGuid,
         ConditionalOperatorType operatorType
        )
    {
        var questionId = new QuestionId(questionIdGuid);

        if (operatorType == ConditionalOperatorType.Between)
        {
            var betweenResult = GetBetweenValues(filter);
            if (betweenResult.IsFailure)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, betweenResult.Errors);
            }

            var (fromElement, toElement) = betweenResult.Value;

            if (!CommonJsonElementMethods.TryGetInt64(fromElement, out var from))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.Value",
                        $"The value '{fromElement.GetRawText()}' is not a valid integer."));
            }

            if (!CommonJsonElementMethods.TryGetInt64(toElement, out var to))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"The value '{toElement.GetRawText()}' is not a valid integer."));
            }

            if (from > to)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"SecondValue {from} must be greater than or equal to Value {to}."));
            }

            query = query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted &&
                a.IdQuestion == questionId &&
                a.ValueInteger >= from &&
                a.ValueInteger <= to));

            return ResultT<IQueryable<SubmissionDomain>>.Success(query);
        }

        if (!CommonJsonElementMethods.TryGetInt64(filter.Value!.Value, out var intValue))
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The value '{filter.Value.Value.GetRawText()}' is not a valid integer."));
        }

        query = operatorType switch
        {
            ConditionalOperatorType.GreaterThan => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger > intValue)),

            ConditionalOperatorType.GreaterThanOrEqual => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger >= intValue)),

            ConditionalOperatorType.LessThan => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger < intValue)),

            ConditionalOperatorType.LessThanOrEqual => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger <= intValue)),

            ConditionalOperatorType.Equals => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger == intValue)),

            ConditionalOperatorType.NotEquals => query.Where(s => !s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueInteger == intValue)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for question type 'InputTypeInteger'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyDecimalFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        Guid questionIdGuid,
        ConditionalOperatorType operatorType)
    {
        var questionId = new QuestionId(questionIdGuid);
        if (operatorType == ConditionalOperatorType.Between)
        {
            var betweenResult = GetBetweenValues(filter);
            if (betweenResult.IsFailure)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, betweenResult.Errors);
            }

            var (fromElement, toElement) = betweenResult.Value;

            if (!CommonJsonElementMethods.TryGetDecimal(fromElement, out var from))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.Value",
                        $"The value '{fromElement.GetRawText()}' is not a valid decimal."));
            }

            if (!CommonJsonElementMethods.TryGetDecimal(toElement, out var to))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"The value '{toElement.GetRawText()}' is not a valid decimal."));
            }

            if (from > to)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"SecondValue {from} must be greater than or equal to Value {to}."));
            }

            query = query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted &&
                a.IdQuestion == questionId &&
                a.ValueDecimal >= from &&
                a.ValueDecimal <= to));

            return ResultT<IQueryable<SubmissionDomain>>.Success(query);
        }
        if (!CommonJsonElementMethods.TryGetDecimal(filter.Value!.Value, out var decimalValue))
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The value '{filter.Value.Value.GetRawText()}' is not a valid decimal number."));
        }


        query = operatorType switch
        {
            ConditionalOperatorType.GreaterThan => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal > decimalValue)),

            ConditionalOperatorType.GreaterThanOrEqual => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal >= decimalValue)),

            ConditionalOperatorType.LessThan => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal < decimalValue)),

            ConditionalOperatorType.LessThanOrEqual => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal <= decimalValue)),

            ConditionalOperatorType.Equals => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal == decimalValue)),

            ConditionalOperatorType.NotEquals => query.Where(s => !s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDecimal == decimalValue)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for question type 'InputTypeDecimal'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private ResultT<IQueryable<SubmissionDomain>> ApplyDateTimeFilter(
        IQueryable<SubmissionDomain> query,
        FiltersForm filter,
        Guid questionIdGuid,
        ConditionalOperatorType operatorType)
    {
        var questionId = new QuestionId(questionIdGuid);
        if (operatorType == ConditionalOperatorType.Between)
        {
            var betweenResult = GetBetweenValues(filter);
            if (betweenResult.IsFailure)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(ResultType.BadRequest, betweenResult.Errors);
            }

            var (fromElement, toElement) = betweenResult.Value;

            if (!CommonJsonElementMethods.TryGetDateTime(fromElement, out var from))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.Value",
                        $"The value '{fromElement.GetRawText()}' is not a valid datetime."));
            }

            if (!CommonJsonElementMethods.TryGetDateTime(toElement, out var to))
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"The value '{toElement.GetRawText()}' is not a valid datetime."));
            }

            if (from > to)
            {
                return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                    ResultType.BadRequest,
                    ResultError.InvalidInput(
                        "Filter.SecondValue",
                        $"SecondValue {from} must be greater than or equal to Value {to}."));
            }

            query = query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted &&
                a.IdQuestion == questionId &&
                a.ValueDateTime >= from &&
                a.ValueDateTime <= to));

            return ResultT<IQueryable<SubmissionDomain>>.Success(query);
        }
        if (!CommonJsonElementMethods.TryGetDateTime(filter.Value!.Value, out var dateTimeValue))
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.Value",
                    $"The value '{filter.Value.Value.GetRawText()}' is not a valid datetime."));
        }


        query = operatorType switch
        {
            ConditionalOperatorType.On => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDateTime == dateTimeValue)),

            ConditionalOperatorType.After => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDateTime > dateTimeValue)),

            ConditionalOperatorType.OnOrAfter => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDateTime >= dateTimeValue)),

            ConditionalOperatorType.Before => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDateTime < dateTimeValue)),

            ConditionalOperatorType.OnOrBefore => query.Where(s => s.SubmissionValues.Any(a =>
                !a.IsDeleted && a.IdQuestion == questionId && a.ValueDateTime <= dateTimeValue)),

            _ => null!
        };

        if (query is null)
        {
            return ResultT<IQueryable<SubmissionDomain>>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.OperatorId",
                    $"The operator '{operatorType}' is not supported for question type 'InputTypeDatetime'."));
        }

        return ResultT<IQueryable<SubmissionDomain>>.Success(query);
    }

    private static bool IsSubmissionField(string key)
    {
        return key.Equals("submittedAt", StringComparison.OrdinalIgnoreCase);
    }


    private static ResultT<Guid> TryGetQuestionId(string key)
    {
        if (!key.StartsWith("q_", StringComparison.OrdinalIgnoreCase))
        {
            return ResultT<Guid>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidInput(
                    "Filter.ColumnKey",
                    $"Column key '{key}' is invalid. Column key should start with 'q_'."));
        }

        var rawQuestionId = key["q_".Length..];

        if (!Guid.TryParse(rawQuestionId, out var questionIdGuid))
        {
            return ResultT<Guid>.FailureT(
                ResultType.BadRequest,
                ResultError.InvalidFormat(
                    "Filter.ColumnKey",
                    $"Column key '{key}' does not contain a valid question id."));
        }

        return questionIdGuid;
    }

    private static string EscapeLikeValue(string value)
    {
        return value
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("_", "[_]");
    }
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
}
