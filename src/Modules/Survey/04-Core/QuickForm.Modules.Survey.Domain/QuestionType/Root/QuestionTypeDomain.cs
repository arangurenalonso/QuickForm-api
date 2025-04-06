using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class QuestionTypeDomain : BaseDomainEntity<QuestionTypeId>
{

    public QuestionTypeKeyNameVO KeyName { get; private set; }

    #region Many to One
    public ICollection<QuestionTypeAttributeDomain> QuestionTypeAttributes { get; private set; } = [];
    #endregion
    private QuestionTypeDomain() { }

    private QuestionTypeDomain(
        QuestionTypeId id,
        QuestionTypeKeyNameVO keyName) : base(id)
    {
        KeyName = keyName;

    }
    public static ResultT<QuestionTypeDomain> Create(
        QuestionTypeId id,
        string keyName
      )
    {
        var keyNameResult = QuestionTypeKeyNameVO.Create(keyName);

        if (keyNameResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors }
                );
            return errorList;
        }
        var domain = new QuestionTypeDomain(id, keyNameResult.Value);

        return domain;
    }
    public static ResultT<QuestionTypeDomain> Create(
            string keyName
        )
        => Create(QuestionTypeId.Create(), keyName);

    public Result Update(
           string keyName
       )
    {
        var keyNameResult = QuestionTypeKeyNameVO.Create(keyName);

        if (keyNameResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { keyNameResult.Errors }
                );
            return errorList;
        }
        KeyName = keyNameResult.Value;
        return Result.Success();
    }
}
