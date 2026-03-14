using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class AuthActionTokenDomain : BaseDomainEntity<AuthActionTokenId>
{
    public UserId IdUser { get; private set; }
    public MasterId IdUserAction { get; private set; }
    public TokenVO Token { get; private set; }
    public bool Used { get; private set; }
    public ExpirationDate ExpiresAt { get; private set; }


    #region One to Many
    public UserDomain User { get; private set; }
    public AuthActionDomain Action { get; private set; }
    #endregion


    private AuthActionTokenDomain() { }

    private AuthActionTokenDomain(
        AuthActionTokenId id, UserId idUser, MasterId idUserAction, TokenVO token, bool used, ExpirationDate expiresAt
        ) : base(id)
    {
        IdUser = idUser;
        IdUserAction = idUserAction;
        Token = token;
        Used = used;
        ExpiresAt = expiresAt;
    }

    public static ResultT<AuthActionTokenDomain> Create(UserId idUser, MasterId idUserAction,
        DateTime expiredDate)
    {

        TokenVO token = idUserAction.Value switch
        {
            var action when action == AuthActionType.RecoveryPassword.GetId() ||
                            action == AuthActionType.EmailConfirmation.GetId()
                => TokenVO.GenerateOtp(),

            _ => TokenVO.GenerateLongToken(32)
        };

        var expiredDateResult = ExpirationDate.Create(expiredDate);

        if ( expiredDateResult.IsFailure)
        {
            return expiredDateResult.Errors;
        }
        return new AuthActionTokenDomain(AuthActionTokenId.Create(),
            idUser,
            idUserAction,
            token,
            false,
            expiredDateResult.Value
            );
    }
    public Result UseToken(DateTime currentDateTime)
    {
        if (Used)
        {
            return ResultError.InvalidOperation("AuthActionToken", "Token has already been used.");
        }

        if (ExpiresAt.IsExpired(currentDateTime))
        {
            return ResultError.InvalidOperation("AuthActionToken", "Token has already expired.");
        }

        Used = true;
        return Result.Success();
    }

}
