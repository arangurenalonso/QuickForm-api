using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class RuleDomain : BaseMasterEntity
{
    public DataTypeId IdDataType { get; private set; }

    #region One to Many
    public DataTypeDomain DataType { get; private set; }
    #endregion

    #region Many to One
    public ICollection<QuestionTypeRuleDomain> QuestionTypeRule { get; private set; } = [];
    #endregion
    public RuleDomain() { }
    private RuleDomain(
            MasterId id,
            DataTypeId idDataType
        ) : base(id) 
    {
        IdDataType = idDataType;
    }
    public static ResultT<RuleDomain> Create(
            DataTypeId idDataType,
            string keyName,
            string? description = null
        )
    {
        var newId = MasterId.Create();
        return Create(newId, idDataType, keyName, description);
    }
    public static ResultT<RuleDomain> Create(MasterId id, DataTypeId idDataType, string keyName, string? description = null)
    {
        var newDomain = new RuleDomain(id, idDataType);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }


    public Result Update(
           string keyName,
           string description,
           DataTypeId idDataType
       )
    {
        IdDataType = idDataType;

        var resultUpdated = base.Update(keyName, description);
        if (resultUpdated.IsFailure)
        {
            return resultUpdated.Errors;
        }
        return Result.Success();
    }



}

