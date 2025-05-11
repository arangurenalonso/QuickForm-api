using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormDomain : BaseDomainEntity<FormId>
{

    public FormNameVO Name { get; private set; }
    public FormDescription Description { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsClosed { get; private set; }
    public DateEnd? DateEnd { get; private set; }
    public CustomerId IdCustomer { get; private set; }

    #region One to Many
    public Customer Customer { get; private set; }
    #endregion
    #region Many to One
    public ICollection<FormSectionDomain> Sections { get; private set; } = [];
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
        IsPublished = false;
        IsClosed = false;
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

        return newForm;
    }
    public Result Update(string name, string? description)
    {
        if (IsPublished)
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
        if (IsPublished)
        {
            return ResultError.InvalidOperation("IsPublished", "Form is already published.");
        }
        IsPublished = true;
        DateEnd = IsPremiun ?DateEnd.NoRestriction():DateEnd.WithRestriction();

        return Result.Success();

    }

    public Result Close()
    {
        if (IsClosed)
        {
            return ResultError.InvalidOperation("IsClose", "Form is already close.");
        }
        IsClosed = true;

        return Result.Success();

    }
}
