using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class AuthActionDomain : BaseDomainEntity<AuthActionId>
{
    public ActionDescriptionVO Description { get; private set; }
    #region Many-to-Many Relationship
    public ICollection<AuthActionTokenDomain> UserActionTokens { get; private set; } = [];

    #endregion

    private AuthActionDomain() { }

    private AuthActionDomain(
        AuthActionId id, ActionDescriptionVO description) : base(id)
    {
        Description = description;

    }

    public static ResultT<AuthActionDomain> Create(
        string description)
    {
        var descriptionResult = ActionDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        return new AuthActionDomain(AuthActionId.Create(), descriptionResult.Value);
    }
    public static ResultT<AuthActionDomain> Create(AuthActionId id, string description)
    {
        var descriptionResult = ActionDescriptionVO.Create(description);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors }
                );
            return errorList;
        }
        return new AuthActionDomain(id, descriptionResult.Value);

    }

    public Result Update(
           string description
       )
    {
        var descriptionResult = ActionDescriptionVO.Create(description);

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
