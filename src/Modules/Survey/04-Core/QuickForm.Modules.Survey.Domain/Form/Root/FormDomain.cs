﻿using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain.Form;
public class FormDomain : BaseDomainEntity<FormId>
{

    public FormNameVO Name { get; private set; }
    public FormDescription Description { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsClosed { get; private set; }
    public DateEnd? DateEnd { get; private set; }

    private FormDomain() { }
    private FormDomain(FormId id, FormNameVO name, FormDescription description) : base(id)
    {
        Name = name;
        Description = description;
        IsPublished = false;
        IsClosed = false;
    }

    public static ResultT<FormDomain> Create(string name, string? description)
    {
        var nameResult = FormNameVO.Create(name);
        var descriptionResult = FormDescription.Create(description);

        if (nameResult.IsFailure || descriptionResult.IsFailure)
        {
            return new ResultErrorList(
                new List<ResultErrorList>() { nameResult.Errors, descriptionResult.Errors }
                );
        }
        var newForm = new FormDomain(FormId.Create(), nameResult.Value, descriptionResult.Value);

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
