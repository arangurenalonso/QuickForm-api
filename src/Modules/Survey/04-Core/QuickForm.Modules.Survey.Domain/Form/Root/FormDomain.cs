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

    public Result ApplySectionsChanges(
            IReadOnlyCollection<(Guid Id, 
                                string Title, 
                                string Description,
                                List<(Guid Id, JsonElement Properties, QuestionTypeDomain QuestionType)> Questions  
                            )> incomingSections
        )
    {

        var guard = EnsureCanUpdate();
        if (guard.IsFailure)
        {
            return guard;
        }

        incomingSections ??= Array.Empty<(Guid Id, string Title, string Description, List<(Guid Id, JsonElement Properties, QuestionTypeDomain QuestionType)> Questions)>();

        var duplicatedIds = incomingSections
            .GroupBy(s => s.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedIds.Count > 0)
        {
            return ResultError.InvalidOperation(
                "DuplicateSectionIds",
                $"Incoming sections contain duplicated Ids: {string.Join(", ", duplicatedIds)}");
        }

        var existingById = Sections
                              .Where(s => !s.IsDeleted) 
                              .ToDictionary(s => s.Id.Value, s => s);

        var incomingIds = incomingSections.Select(s => s.Id).ToHashSet();
        foreach (var section in Sections.Where(s => !s.IsDeleted && !incomingIds.Contains(s.Id.Value)))
        {
            section.MarkDeleted();
        }

        var order = 1;
        foreach (var dto in incomingSections)
        {
            if (existingById.TryGetValue(dto.Id, out var existing))
            {
                var updateResult = existing.Update(order, dto.Title, dto.Description);
                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
                var questionChangesResult = existing.ApplyQuestionChanges(dto.Questions);
                if (questionChangesResult.IsFailure)
                {
                    return questionChangesResult;
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
                    order);

                if (createResult.IsFailure)
                {
                    return Result.Failure(ResultType.DomainValidation, createResult.Errors);
                }

                var questionChangesResult = createResult.Value.ApplyQuestionChanges(dto.Questions);
                if (questionChangesResult.IsFailure)
                {
                    return questionChangesResult.Errors;
                }
                Sections.Add(createResult.Value);
            }

            order++;
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
