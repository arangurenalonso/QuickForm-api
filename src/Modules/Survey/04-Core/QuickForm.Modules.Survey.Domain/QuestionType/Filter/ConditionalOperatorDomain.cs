using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class ConditionalOperatorDomain : BaseMasterEntity
{
    #region Many to One
    public ICollection<QuestionTypeFilterDomain> QuestionTypeFilter { get; private set; } = [];
    #endregion
    private ConditionalOperatorDomain() { }

    private ConditionalOperatorDomain(
        MasterId id
        ) : base(id)
    {
    }
    public static ResultT<ConditionalOperatorDomain> Create(
        MasterId id,
        string keyName,
        string description
      )
    {

        var newDomain = new ConditionalOperatorDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }
    public static ResultT<ConditionalOperatorDomain> Create(
            string keyName,
            string description
    )
        => Create(MasterId.Create(), keyName, description);

    public override Result Update(
           string keyName,
           string? description = null
       )
    {

        var resultUpdated = base.Update(keyName, description);
        if (resultUpdated.IsFailure)
        {
            return resultUpdated.Errors;
        }
        return Result.Success();
    }
}

