using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormStatusDomain : BaseStatusEntity
{


    #region Many-to-One Relationship
    public ICollection<FormDomain> Forms { get; private set; } = [];

    #endregion



    private FormStatusDomain() { }
    private FormStatusDomain(MasterId id) : base(id) { }

    public static ResultT<FormStatusDomain> Create(
            string keyName,
            string color,
            string? icon = null,
            string? description = null
        )
    {
        var newDomain = new FormStatusDomain();
        var masterUpdateBase = new StatusPropertiesDto(keyName, description,color,icon);
        var result = newDomain.SetStatusProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }

    public Result Update(
            string keyName,
            string color,
            string? icon = null,
            string? description = null
        )
    {
        var masterUpdateBase = new StatusPropertiesDto(keyName, description,color,icon);
        return SetStatusProperties(masterUpdateBase);
    }


}
