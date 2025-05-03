using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class AttributeDomain : BaseDomainEntity<AttributeId>
{

    public AttributeKeyNameVO KeyName { get; private set; }
    public AttributeDescriptionVO Description { get; private set; }
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
        AttributeId id, 
        AttributeKeyNameVO keyName,
        AttributeDescriptionVO description,
        DataTypeId idDataType,
        bool mustBeUnique) : base(id)
    {
        KeyName = keyName;
        Description = description;
        IdDataType = idDataType;
        MustBeUnique = mustBeUnique;
    }
    public static ResultT<AttributeDomain> Create(
        AttributeId id,
        string keyName,
        string description,
        DataTypeId idDataType,
        bool mustBeUnique
      )
    {
        var keyNameResult = AttributeKeyNameVO.Create(keyName);
        var descriptionResult = AttributeDescriptionVO.Create(description);
        if (keyNameResult.IsFailure || descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        var domain = new AttributeDomain(id, keyNameResult.Value, descriptionResult.Value, idDataType, mustBeUnique);

        return domain;
    }
    public static ResultT<AttributeDomain> Create(
            string keyName,
            string description,
            DataTypeId idDataType,
            bool mustBeUnique
    )
        => Create(AttributeId.Create(), keyName, description, idDataType, mustBeUnique);

    public Result Update(
           string keyName,
           string description,
           DataTypeId idDataType,
           bool mustBeUnique
       )
    {
        var keyNameResult = AttributeKeyNameVO.Create(keyName);
        var descriptionResult = AttributeDescriptionVO.Create(description);

        if (keyNameResult.IsFailure || descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        KeyName = keyNameResult.Value;
        Description = descriptionResult.Value;
        IdDataType = idDataType;
        MustBeUnique = mustBeUnique;

        return Result.Success();
    }
}
