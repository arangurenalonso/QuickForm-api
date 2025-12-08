using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class AttributeDomain : BaseMasterEntity
{
    public DataTypeId IdDataType { get; private set; }
    public bool MustBeUnique { get; private set; }

    #region One to Many
    public DataTypeDomain DataType { get; private set; }
    #endregion

    #region Many to One
    public ICollection<QuestionTypeAttributeDomain> QuestionTypeAttributes { get; private set; } = [];
    #endregion
    private AttributeDomain() { }

    private AttributeDomain(
        MasterId id, 
        DataTypeId idDataType,
        bool mustBeUnique) : base(id)
    {
        IdDataType = idDataType;
        MustBeUnique = mustBeUnique;
    }
    public static ResultT<AttributeDomain> Create(
        MasterId id,
        string keyName,
        string description,
        DataTypeId idDataType,
        bool mustBeUnique
      )
    {

        var newDomain = new AttributeDomain(id, idDataType, mustBeUnique);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }
    public static ResultT<AttributeDomain> Create(
            string keyName,
            string description,
            DataTypeId idDataType,
            bool mustBeUnique
    )
        => Create(MasterId.Create(), keyName, description, idDataType, mustBeUnique);

    public Result Update(
           string keyName,
           string description,
           DataTypeId idDataType,
           bool mustBeUnique
       )
    {
        IdDataType = idDataType;
        MustBeUnique = mustBeUnique;

        var resultUpdated = base.Update(keyName, description);
        if (resultUpdated.IsFailure)
        {
            return resultUpdated.Errors;
        }   
        return Result.Success();
    }
}
