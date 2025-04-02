using System.ComponentModel.DataAnnotations;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class AttributeDomain : BaseDomainEntity<AttributeId>
{

    public AttributeKeyNameVO KeyName { get; private set; }
    public AttributeDescriptionVO Description { get; private set; }
    public DataTypeId IdDataType { get; private set; }

    #region One to Many
    public DataTypeDomain DataType { get; private set; }
    #endregion

    private AttributeDomain() { }

    private AttributeDomain(
        AttributeId id, 
        AttributeKeyNameVO keyName,
        AttributeDescriptionVO description,
        DataTypeId idDataType) : base(id)
    {
        KeyName = keyName;
        Description = description;
        IdDataType = idDataType;
    }
    public static ResultT<AttributeDomain> Create(
        AttributeId id,
        string keyName,
        string description,
        DataTypeId idDataType
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
        var domain = new AttributeDomain(id, keyNameResult.Value, descriptionResult.Value, idDataType);

        return domain;
    }
    public static ResultT<AttributeDomain> Create(
            string keyName,
            string description,
            DataTypeId idDataType
    )
        => Create(AttributeId.Create(), keyName, description, idDataType);

    public Result Update(
           string keyName,
           string description,
           DataTypeId idDataType
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
        return Result.Success();
    }
}
