using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;
public sealed class UserDomain : BaseDomainEntity<UserId>
{
    public EmailVO Email { get; private set; }
    public PasswordVO PasswordHash { get; private set; }
    public bool IsPasswordChanged { get; private set; }
    public bool IsEmailVerify { get; private set; }

    #region Many-to-Many Relationship
    public ICollection<AuthActionTokenDomain> AuthActionTokens { get; private set; } = [];
    public ICollection<UserRoleDomain> UserRole { get; private set; } = [];
    #endregion
    public UserDomain() { }

    private UserDomain(
        UserId id,
        EmailVO email,
        PasswordVO passwordHash) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
    }

    public static ResultT<UserDomain> Create(
            string email,
            string password,
            IPasswordHashingService passwordHashingService,
            DateTime currentDateTime,
            RoleDomain roleDomain
        )
    {
        var emailResult = EmailVO.Create(email);
        var passwordResult = PasswordVO.Create(password, passwordHashingService);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { emailResult.Errors, passwordResult.Errors }
                );
            return errorList;
        }
        var newUser = new UserDomain(UserId.Create(), emailResult.Value, passwordResult.Value);
        newUser.AddRole(roleDomain);
        var idAuthActionEmailVerificacion = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new MasterId(idAuthActionEmailVerificacion);

        newUser.AddAction(idAuthAction, currentDateTime);
        newUser.RaiseDomainEvents(new UserRegisteredDomainEvent(newUser.Id));

        return newUser;
    }

    public Result ChangePassword(
        string newPassword, 
        IPasswordHashingService? passwordHashingService=null)
    {
        var passwordResult = PasswordVO.Create(newPassword, passwordHashingService);
        if (passwordResult.IsFailure)
        {
            return passwordResult.Errors;
        }
        PasswordHash = passwordResult.Value;
        return Result.Success();


    }
    public Result AddAction(
        MasterId idAuthAction,
        DateTime currentDateTime)
    {
        var existCurrentActionInProgress = AuthActionTokens.Where(x =>
                                                        x.IdUserAction == idAuthAction &&
                                                        x.ExpiresAt.Value >= currentDateTime
                                                        ).ToList();

        if (existCurrentActionInProgress.Count == 0)
        {
            var expiredDate = currentDateTime.AddMinutes(30);
            var userActionTokenDomainResult = AuthActionTokenDomain.Create(Id, idAuthAction, expiredDate);
            if (userActionTokenDomainResult.IsFailure)
            {
                return userActionTokenDomainResult.Errors;
            }
            AuthActionTokens.Add(userActionTokenDomainResult.Value);
        }


        RaiseDomainEvents(new AuthActionDomainEvent(Id, idAuthAction));

        return Result.Success();
    }
    public Result AddRole(RoleDomain role)
    {
        if (role == null)
        {
            return ResultError.NullValue("Role","Role cannot be null.");
        }

        if (UserRole.Any(ur => ur.IdRole == role.Id))
        {
            return ResultError.DuplicateValueAlreadyInUse("Role", "User already has this role assigned.");
        }

        var userRoleResult = UserRoleDomain.Create(Id, role.Id);

        if (userRoleResult.IsFailure)
        {
            return userRoleResult.Errors;
        }

        UserRole.Add(userRoleResult.Value);

        return Result.Success();
    }

    public void ConfirmEmail()
    {
        IsEmailVerify = true;
    }
    
}
