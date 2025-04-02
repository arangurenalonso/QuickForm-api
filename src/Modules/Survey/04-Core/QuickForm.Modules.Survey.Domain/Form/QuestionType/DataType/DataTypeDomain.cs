using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class DataTypeDomain : BaseDomainEntity<DataTypeId>
{

    public DataTypeDescriptionVO Description { get; private set; }

    #region Many to One
    public ICollection<AttributeDomain> Attributes { get; private set; } = [];
    #endregion
    private DataTypeDomain() { }

    private DataTypeDomain(
        DataTypeId id, DataTypeDescriptionVO description) : base(id)
    {
        Description = description;

    }
    public static ResultT<DataTypeDomain> Create(
        DataTypeId id,
        string description
      )
    {
        var descriptionResult = DataTypeDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        var domain = new DataTypeDomain(id, descriptionResult.Value);

        return domain;
    }
    public static ResultT<DataTypeDomain> Create(
    DataTypeDescriptionVO description
        )
        => Create(DataTypeId.Create(), description);

    public Result Update(
           string description
       )
    {
        var descriptionResult = DataTypeDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        Description = descriptionResult.Value;
        return Result.Success();
    }
}
