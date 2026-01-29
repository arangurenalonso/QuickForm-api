using System.Text.Json;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormDomain : BaseDomainEntity<FormId>
{

    public FormNameVO Name { get; private set; }
    public FormDescription Description { get; private set; }
    public DateEnd? DateEnd { get; private set; }
    public CustomerId IdCustomer { get; private set; }
    public MasterId IdStatus { get; private set; }

    #region One to Many
    public Customer Customer { get; private set; }
    public FormStatusDomain Status { get; private set; }
    #endregion
    #region Many to One
    public ICollection<FormSectionDomain> Sections { get; private set; } = [];
    public ICollection<FormStatusHistoryDomain> StatusHistory { get; private set; } = [];
    
    #endregion
    private FormDomain() { }
    private FormDomain(
        FormId id, 
        FormNameVO name, 
        FormDescription description,
        CustomerId idCustomer) : base(id)
    {
        Name = name;
        Description = description;
        IdStatus = new MasterId(FormStatusType.Draft.GetId());
        IdCustomer = idCustomer;
    }

    public static ResultT<FormDomain> Create(string name, string? description,Customer customer)
    {
        var nameResult = FormNameVO.Create(name);
        var descriptionResult = FormDescription.Create(description);

        if (nameResult.IsFailure || descriptionResult.IsFailure)
        {
            return new ResultErrorList(
                new List<ResultErrorList>() { nameResult.Errors, descriptionResult.Errors }
                );
        }
        var newForm = new FormDomain(FormId.Create(), nameResult.Value, descriptionResult.Value,customer.Id);
        
        var registerStatusHistory = newForm.RegisterStatusHistory();
        if (registerStatusHistory.IsFailure)
        {
            return registerStatusHistory.Errors;
        }

        return newForm;
    }
    
    public Result ApplySectionsChanges(
            IReadOnlyCollection<(Guid Id, 
                                string Title, 
                                string Description,
                                List<(
                                    Guid Id, 
                                    JsonElement Properties,
                                    Dictionary<string, ValidationRule>? Rules, 
                                    QuestionTypeDomain QuestionType
                                    )> Questions  
                            )> incomingSections
        )
    {

        var guard = EnsureCanUpdate();
        if (guard.IsFailure)
        {
            return guard;
        }

        incomingSections ??= Array.Empty<(Guid Id, string Title, string Description, List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions)>();

        
        var resultValidateSections = ValidateIncomingSectionsAndQuestions(incomingSections);
        if (resultValidateSections.IsFailure)
        {
            return resultValidateSections;
        }

        var resultUpdateBasicSectionsInformation = UpdateBasicSectionsInformation(incomingSections);
        if (resultUpdateBasicSectionsInformation.IsFailure)
        {
            return resultUpdateBasicSectionsInformation;
        }

        var resultRelocateExistingQuestionsToSections = RelocateExistingQuestionsToSections(incomingSections);
        if (resultRelocateExistingQuestionsToSections.IsFailure)
        {
            return resultRelocateExistingQuestionsToSections;
        }

        var resultUpdateQuestionsSections = UpdateQuestionsSections(incomingSections);
        if (resultUpdateQuestionsSections.IsFailure)
        {
            return resultUpdateQuestionsSections;
        }

        return Result.Success();
    }
    private Result EnsureCanUpdate()
    {
        var allowed = new[] { FormStatusType.Draft, FormStatusType.Paused };
        var isAllowed = allowed.Any(s => s.GetId() == IdStatus.Value);

        if (isAllowed)
        {
            return Result.Success();

        }

        var allowedNames = string.Join(", ", allowed);
        return ResultError.InvalidOperation(
            "FormStatus",
            $"The form '{Name.Value}' cannot be updated because its current status is '{Status.KeyName}'. Allowed: {allowedNames}."
        );
    }
    private Result ValidateIncomingSectionsAndQuestions(
        IReadOnlyCollection<(Guid Id,
                             string Title,
                             string Description,
                             List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions
                            )> incomingSections
    )
    {
        var duplicatedSectionIds = incomingSections
            .GroupBy(s => s.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedSectionIds.Count > 0)
        {
            return ResultError.InvalidOperation(
                "DuplicateSectionIds",
                $"Incoming sections contain duplicated Ids: {string.Join(", ", duplicatedSectionIds)}");
        }

        var incomingQuestionIds = incomingSections
            .SelectMany(s => (s.Questions ?? []).Select(q => q.Id))
            .ToList();

        var duplicatedQuestionIds = incomingQuestionIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedQuestionIds.Count > 0)
        {
            return ResultError.InvalidOperation(
                "DuplicateQuestionIds",
                $"Incoming questions contain duplicated Ids across sections: {string.Join(", ", duplicatedQuestionIds)}");
        }

        return Result.Success();
    }

    private Result UpdateBasicSectionsInformation(
            IReadOnlyCollection<(Guid Id,
                                string Title,
                                string Description,
                                List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions
                            )> incomingSections
        )
    {
        var activeSectionsById = Sections
                              .Where(s => !s.IsDeleted)
                              .ToDictionary(s => s.Id.Value, s => s);

        var incomingSectionIds = incomingSections.Select(s => s.Id).ToHashSet();
        foreach (var section in Sections.Where(s => !s.IsDeleted && !incomingSectionIds.Contains(s.Id.Value)))
        {
            section.MarkDeleted();
        }

        var orderSection = 1;
        foreach (var dto in incomingSections)
        {
            if (activeSectionsById.TryGetValue(dto.Id, out var existing))
            {
                var updateResult = existing.Update(orderSection, dto.Title, dto.Description);
                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
            }
            else
            {
                var idSection = new FormSectionId(dto.Id);

                var createResult = FormSectionDomain.Create(
                    idSection,
                    Id,
                    dto.Title,
                    dto.Description,
                    orderSection);

                if (createResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, createResult.Errors);
                }
                Sections.Add(createResult.Value);
                activeSectionsById[idSection.Value] = createResult.Value;
            }

            orderSection++;
        }
        return Result.Success();
    }

    private Result RelocateExistingQuestionsToSections(
            IReadOnlyCollection<(Guid Id,
                                string Title,
                                string Description,
                                List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions
                            )> incomingSections
        )
    {
        incomingSections ??= Array.Empty<(Guid Id, string Title, string Description, List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions)>();

        var targetSectionsById = Sections
            .Where(s => !s.IsDeleted)
            .ToDictionary(s => s.Id.Value, s => s);

        var allSectionsById = Sections
            .ToDictionary(s => s.Id.Value, s => s);


        var activeQuestionsById = Sections
                                    .SelectMany(s => s.Questions.Where(q => !q.IsDeleted))
                                    .ToDictionary(q => q.Id.Value, q => q);

        foreach (var incomingSection in incomingSections)
        {
            if (!targetSectionsById.TryGetValue(incomingSection.Id, out var targetSection))
            {
                return ResultError.InvalidOperation(
                    "SectionNotFound",
                    $"Incoming section '{incomingSection.Id}' was not found among active sections.");
            }

            foreach (var incomingQuestion in incomingSection.Questions ?? [])
            {
                if (!activeQuestionsById.TryGetValue(incomingQuestion.Id, out var existingQuestion))
                {
                    continue;
                }

                var currentSectionId = existingQuestion.IdFormSection.Value;

                if (currentSectionId == targetSection.Id.Value)
                {
                    continue;
                }

                if (allSectionsById.TryGetValue(currentSectionId, out var currentSection))
                {
                    currentSection.Questions.Remove(existingQuestion);
                }

                var moveResult = existingQuestion.MoveToSection(targetSection.Id);
                if (moveResult.IsFailure)
                {
                    return moveResult;
                }
                if (!targetSection.Questions.Any(q => q.Id.Value == existingQuestion.Id.Value))
                {
                    targetSection.Questions.Add(existingQuestion);
                }
            }
        }
        return Result.Success();
    }

    private Result UpdateQuestionsSections(
            IReadOnlyCollection<(Guid Id,
                                string Title,
                                string Description,
                                List<(Guid Id, JsonElement Properties, Dictionary<string, ValidationRule>? Rules, QuestionTypeDomain QuestionType)> Questions
                            )> incomingSections
        )
    {
        var activeSectionsById = Sections
                              .Where(s => !s.IsDeleted)
                              .ToDictionary(s => s.Id.Value, s => s);
        foreach (var dto in incomingSections)
        {

            if (!activeSectionsById.TryGetValue(dto.Id, out var existing))
            {
                return ResultError.InvalidOperation(
                    "SectionNotFound",
                    $"Incoming section '{dto.Id}' was not found among active sections.");
            }

            var questionChangesResult = existing.ApplyQuestionChanges(dto.Questions ?? []);
            if (questionChangesResult.IsFailure)
            {
                return questionChangesResult;
            }
        }
        return Result.Success();
    }
    
    
    public Result Update(string name, string? description)
    {

        var guard = EnsureCanUpdate();
        if (guard.IsFailure)
        {
            return guard;
        }

        var nameResult = FormNameVO.Create(name);
        var descriptionResult = FormDescription.Create(description);

        if (nameResult.IsFailure || descriptionResult.IsFailure)
        {
            return  new ResultErrorList(
                new List<ResultErrorList>() { nameResult.Errors, descriptionResult.Errors }
                );
        }
        Name = nameResult.Value;
        Description = descriptionResult.Value;

        return Result.Success();

    }
    public Result Publish(bool IsPremiun)
    {
        var isPublished = IdStatus.Value == FormStatusType.Published.GetId();
        if (isPublished)
        {
            return ResultError.InvalidOperation("IsPublished", "Form is already published.");
        }

        var registerStatusHistory = RegisterStatusHistory();
        if (registerStatusHistory.IsFailure)
        {
            return registerStatusHistory.Errors;
        }

        IdStatus = new MasterId(FormStatusType.Published.GetId());
        DateEnd = IsPremiun ?DateEnd.NoRestriction():DateEnd.WithRestriction();

        return Result.Success();

    }
    public Result RegisterStatusHistory()
    {
        var formStatusHistory = FormStatusHistoryDomain.Create(Id, IdStatus);
        if (formStatusHistory.IsFailure)
        {
            return formStatusHistory.Errors;
        }
        StatusHistory.Add(formStatusHistory.Value);
        return Result.Success();
    }
    public Result Close()
    {
        var isClosed = IdStatus.Value == FormStatusType.Closed.GetId(); 
        if (isClosed)
        {
            return ResultError.InvalidOperation("IsClose", "Form is already close.");
        }
        var registerStatusHistory = RegisterStatusHistory();
        if (registerStatusHistory.IsFailure)
        {
            return registerStatusHistory.Errors;
        }
        IdStatus = new MasterId(FormStatusType.Closed.GetId());

        return Result.Success();

    }
}
