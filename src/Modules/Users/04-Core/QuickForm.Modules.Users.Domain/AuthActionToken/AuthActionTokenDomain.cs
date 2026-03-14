using System.ComponentModel.DataAnnotations.Schema;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public class AuthActionTokenDomain : BaseDomainEntity<AuthActionTokenId>
{
    public UserId IdUser { get; private set; }
    public MasterId IdUserAction { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public bool Used { get; private set; }
    public ExpirationDate ExpiresAt { get; private set; }

    [NotMapped]
    public string? PlainTextToken { get; private set; }

    #region One to Many
    public UserDomain User { get; private set; } = null!;
    public AuthActionDomain Action { get; private set; } = null!;
    #endregion

    private AuthActionTokenDomain() { }

    private AuthActionTokenDomain(
        AuthActionTokenId id,
        UserId idUser,
        MasterId idUserAction,
        string tokenHash,
        bool used,
        ExpirationDate expiresAt,
        string? plainTextToken
    ) : base(id)
    {
        IdUser = idUser;
        IdUserAction = idUserAction;
        TokenHash = tokenHash;
        Used = used;
        ExpiresAt = expiresAt;
        PlainTextToken = plainTextToken;
    }

    public static ResultT<AuthActionTokenDomain> Create(
        UserId idUser,
        MasterId idUserAction,
        DateTime expiredDate,
        IAuthActionTokenHashingService tokenHashingService)
    {
        TokenVO token = idUserAction.Value switch
        {
            var action when action == AuthActionType.RecoveryPassword.GetId() ||
                            action == AuthActionType.EmailConfirmation.GetId()
                => TokenVO.GenerateOtp(),

            _ => TokenVO.GenerateLongToken(32)
        };

        var expiredDateResult = ExpirationDate.Create(expiredDate);
        if (expiredDateResult.IsFailure)
        {
            return expiredDateResult.Errors;
        }

        var tokenHashResult = tokenHashingService.Hash(token.Value);
        if (tokenHashResult.IsFailure)
        {
            return tokenHashResult.Errors;
        }

        return new AuthActionTokenDomain(
            AuthActionTokenId.Create(),
            idUser,
            idUserAction,
            tokenHashResult.Value,
            false,
            expiredDateResult.Value,
            token.Value
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

    public void Revoke()
    {
        Used = true;
    }

    public void ClearPlainTextToken()
    {
        PlainTextToken = null;
    }
}
