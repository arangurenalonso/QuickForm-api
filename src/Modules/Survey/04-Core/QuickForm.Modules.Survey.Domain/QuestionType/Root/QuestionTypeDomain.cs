using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeDomain : BaseDomainEntity<QuestionTypeId>
{

    public QuestionTypeKeyNameVO KeyName { get; private set; }
    public DescriptionVO Description { get; private set; }
    public DataTypeId IdDataType { get; private set; }
    #region One to Many
    public DataTypeDomain DataType { get; private set; }
    #endregion

    #region Many to One
    public ICollection<QuestionDomain> Questions { get; private set; } = [];
    public ICollection<QuestionTypeAttributeDomain> QuestionTypeAttributes { get; private set; } = [];
    public ICollection<QuestionTypeRuleDomain> QuestionTypeRules { get; private set; } = [];
    public ICollection<QuestionTypeFilterDomain> QuestionTypeFilter { get; private set; } = [];
    #endregion
    private QuestionTypeDomain() { }

    private QuestionTypeDomain(
        QuestionTypeId id,
        QuestionTypeKeyNameVO keyName,
        DescriptionVO description,
        DataTypeId dataTypeId
        ) : base(id)
    {
        KeyName = keyName;
        Description = description;
        IdDataType = dataTypeId;
    }
    public static ResultT<QuestionTypeDomain> Create(
        QuestionTypeId id,
        string keyName,
        string description,
        DataTypeId dataTypeId

      )
    {
        var keyNameResult = QuestionTypeKeyNameVO.Create(keyName);

        var descriptionResult = DescriptionVO.Create(description);
        if (descriptionResult.IsFailure || keyNameResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        var domain = new QuestionTypeDomain(id, keyNameResult.Value, descriptionResult.Value, dataTypeId);

        return domain;
    }
    public static ResultT<QuestionTypeDomain> Create(
            string keyName,
            string description,
            DataTypeId dataTypeId
        )
        => Create(QuestionTypeId.Create(), keyName, description, dataTypeId);

    public Result Update(
           string keyName,
              string description
       )
    {
        var keyNameResult = QuestionTypeKeyNameVO.Create(keyName);
        var descriptionResult = DescriptionVO.Create(description);

        if (keyNameResult.IsFailure || descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors, descriptionResult.Errors }
                );
            return errorList;
        }
        KeyName = keyNameResult.Value;
        return Result.Success();
    }
}
