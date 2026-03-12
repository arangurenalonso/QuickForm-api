using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class UiControlTypeDomain : BaseMasterEntity
{
    #region Many to One
    public ICollection<QuestionTypeFilterDomain> QuestionTypeFilter { get; private set; } = [];
    #endregion
    private UiControlTypeDomain() { }

    private UiControlTypeDomain(
        MasterId id
        ) : base(id)
    {
    }
    public static ResultT<UiControlTypeDomain> Create(
        MasterId id,
        string keyName,
        string description
      )
    {

        var newDomain = new UiControlTypeDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }
    public static ResultT<UiControlTypeDomain> Create(
            string keyName,
            string description
    )
        => Create(MasterId.Create(), keyName, description);

    public override Result Update(
           string keyName,
           string? description = null,
           int? order = null
       )
    {

        var resultUpdated = base.Update(keyName, description, order);
        if (resultUpdated.IsFailure)
        {
            return resultUpdated.Errors;
        }
        return Result.Success();
    }
}
