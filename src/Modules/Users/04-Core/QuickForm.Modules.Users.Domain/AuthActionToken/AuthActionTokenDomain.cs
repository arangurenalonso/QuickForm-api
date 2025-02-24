using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class AuthActionTokenDomain : BaseDomainEntity<AuthActionTokenId>
{
    public UserId IdUser { get; private set; }
    public AuthActionId IdUserAction { get; private set; }
    public TokenVO Token { get; private set; }
    public bool Used { get; private set; }
    public ExpirationDate ExpiresAt { get; private set; }


    #region One to Many
    public UserDomain User { get; private set; }
    public AuthActionDomain Action { get; private set; }
    #endregion


    private AuthActionTokenDomain() { }

    private AuthActionTokenDomain(
        AuthActionTokenId id, UserId idUser, AuthActionId idUserAction, TokenVO token, bool used, ExpirationDate expiresAt
        ) : base(id)
    {
        IdUser = idUser;
        IdUserAction = idUserAction;
        Token = token;
        Used = used;
        ExpiresAt = expiresAt;
    }

    public static ResultT<AuthActionTokenDomain> Create(UserId idUser, AuthActionId idUserAction,
        DateTime expiredDate)
    {
        var tokenResult = TokenVO.GenerateNewToken();
        var expiredDateResult = ExpirationDate.Create(expiredDate);

        if ( expiredDateResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { expiredDateResult.Errors }
                );
            return errorList;
        }
        return new AuthActionTokenDomain(AuthActionTokenId.Create(),
            idUser,
            idUserAction,
            tokenResult,
            false,
            expiredDateResult.Value
            );
    }
    public void UseToken()
    {
        Used = true;
    }


}
