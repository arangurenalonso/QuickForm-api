using System.Text.Json;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormSectionDomain : BaseDomainEntity<FormSectionId>
{

    public FormId IdForm { get; private set; }
    public FormSectionsDescription Description { get; private set; }
    public FormSectionTitle Title { get; private set; }
    public int SortOrder { get; private set; }


    #region One To Many
    public FormDomain Form { get; private set; }
    #endregion
    #region Many to One
    public ICollection<QuestionDomain> Questions { get; private set; } = [];
    #endregion

    private FormSectionDomain() { }
    private FormSectionDomain(
        FormSectionId id, 
        FormId idForm,
        FormSectionTitle title,
        FormSectionsDescription description,
        int sortOrder) : base(id)
    {
        IdForm = idForm;
        SortOrder = sortOrder;
        Title = title;
        Description = description;
    }

    public static ResultT<FormSectionDomain> Create(
            FormSectionId id,
            FormId idForm, 
            string title,
            string description,
            int sortOrder
        )
    {
        var titleResult = FormSectionTitle.Create(title);
        var descriptionResult = FormSectionsDescription.Create(description);
        if (titleResult.IsFailure || descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { titleResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        
        var newDomain = new FormSectionDomain(id, idForm, titleResult.Value, descriptionResult.Value, sortOrder);

        return newDomain;
    }
    public Result Update(
        int sortOrder,
        string title,
        string description)
    {

        var titleResult = FormSectionTitle.Create(title);
        var descriptionResult = FormSectionsDescription.Create(description);
        if (titleResult.IsFailure || descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { titleResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        SortOrder = sortOrder;
        Title=titleResult.Value;
        Description=descriptionResult.Value;

        return Result.Success();
    }
    public Result ApplyQuestionChanges(
        IReadOnlyCollection<(Guid Id, JsonElement Properties, JsonElement Rules, QuestionTypeDomain QuestionType)> incomingQuestions
        )
    {
        incomingQuestions ??= Array.Empty<(Guid Id, JsonElement Properties, JsonElement Rules, QuestionTypeDomain QuestionType)>();
        var duplicatedIds = incomingQuestions
                                  .GroupBy(q => q.Id)
                                  .Where(g => g.Count() > 1)
                                  .Select(g => g.Key)
                                  .ToList();

        if (duplicatedIds.Count > 0)
        {
            return ResultError.InvalidOperation(
                "DuplicateQuestionIds",
                $"Incoming questions contain duplicated Ids: {string.Join(", ", duplicatedIds)}");
        }
        var existingById = Questions
                            .Where(q => !q.IsDeleted) 
                            .ToDictionary(q => q.Id.Value, q => q);

        var incomingIds = incomingQuestions.Select(q => q.Id).ToHashSet();

        foreach (var q in Questions.Where(q => !q.IsDeleted && !incomingIds.Contains(q.Id.Value)))
        {
            q.MarkDeleted();
        }

        var order = 1;
        foreach (var dto in incomingQuestions)
        {
            if (existingById.TryGetValue(dto.Id, out var existing))
            {
                var updateResult = existing.UpdateSortOrder(order);

                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
                var applyAttributesResult = existing.ApplyAttributeChanges(dto.Properties, dto.QuestionType);
                if (applyAttributesResult.IsFailure)
                {
                    return applyAttributesResult;
                }
                var applyRulesResult = existing.ApplyRuleChanges(dto.Rules, dto.QuestionType);
                if (applyRulesResult.IsFailure)
                {
                    return applyRulesResult;
                }
            }
            else
            {
                var questionId = new QuestionId(dto.Id);

                var createResult = QuestionDomain.Create(
                    questionId,
                    Id,                  
                    dto.QuestionType.Id, 
                    order
                );

                if (createResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, createResult.Errors);
                }
                var applyAttributesResult = createResult.Value.ApplyAttributeChanges(dto.Properties, dto.QuestionType);
                if (applyAttributesResult.IsFailure)
                {
                    return applyAttributesResult;
                }
                var applyRulesResult = createResult.Value.ApplyRuleChanges(dto.Rules, dto.QuestionType);
                if (applyRulesResult.IsFailure)
                {
                    return applyRulesResult;
                }

                Questions.Add(createResult.Value);
            }

            order++;
        }
        return Result.Success();
    }
}
