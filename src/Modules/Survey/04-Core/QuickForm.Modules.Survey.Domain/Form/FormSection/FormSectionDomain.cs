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
}
