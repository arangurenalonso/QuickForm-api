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
    public Result Update(string name, string? description)
    {
        var isPublished = IdStatus.Value == FormStatusType.Published.GetId();
        if (isPublished)
        {
            return  ResultError.InvalidOperation("IsPublished", "Form is published, cannot be updated.");
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
