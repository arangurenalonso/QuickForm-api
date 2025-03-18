using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;
public sealed class UserDomain : BaseDomainEntity<UserId>
{
    public EmailVO Email { get; private set; }
    public PasswordVO PasswordHash { get; private set; }
    public NameVO Name { get; private set; }
    public LastNameVO? LastName { get; private set; }
    public bool IsPasswordChanged { get; private set; }
    public bool IsEmailVerify { get; private set; }

    #region Many-to-Many Relationship
    public ICollection<AuthActionTokenDomain> AuthActionTokens { get; private set; } = [];
    public ICollection<UserRoleDomain> UserRole { get; private set; } = [];
    #endregion
    public UserDomain() { }

    private UserDomain(
        UserId id,
        NameVO name,
        LastNameVO lastName,
        EmailVO email,
        PasswordVO passwordHash) : base(id)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static ResultT<UserDomain> Create(
            string firstName,
            string? lastName,
            string email,
            string password,
            IPasswordHashingService passwordHashingService,
            DateTime currentDateTime
        )
    {
        var emailResult = EmailVO.Create(email);
        var nameResult = NameVO.Create(firstName);
        var lastNameResult = LastNameVO.Create(lastName);
        var passwordResult = PasswordVO.Create(password, passwordHashingService);

        if (emailResult.IsFailure || nameResult.IsFailure || lastNameResult.IsFailure || passwordResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { emailResult.Errors, nameResult.Errors, lastNameResult.Errors, passwordResult.Errors }
                );
            return errorList;
        }
        var newUser = new UserDomain(UserId.Create(), nameResult.Value, lastNameResult.Value, emailResult.Value, passwordResult.Value);

        var idAuthActionEmailVerificacion = AuthActionType.EmailConfirmation.GetId();
        var idAuthAction = new AuthActionId(idAuthActionEmailVerificacion);

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
        AuthActionId idAuthAction,
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

    public void ConfirmEmail()
    {
        IsEmailVerify = true;
    }
    
}
