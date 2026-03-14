using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;

public sealed class UserDomain : BaseDomainEntity<UserId>
{
    public EmailVO Email { get; private set; }
    public PasswordVO PasswordHash { get; private set; }
    public bool IsPasswordChanged { get; private set; }
    public bool IsEmailVerify { get; private set; }

    public ICollection<AuthActionTokenDomain> AuthActionTokens { get; private set; } = [];
    public ICollection<UserRoleDomain> UserRole { get; private set; } = [];

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
        IAuthActionTokenHashingService tokenHashingService,
        DateTime currentDateTime,
        RoleDomain roleDomain
    )
    {
        var emailResult = EmailVO.Create(email);
        var passwordResult = PasswordVO.CreateFromPlainText(password, passwordHashingService);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            return new ResultErrorList(
                new List<ResultErrorList> { emailResult.Errors, passwordResult.Errors }
            );
        }

        var newUser = new UserDomain(UserId.Create(), emailResult.Value, passwordResult.Value);

        var addRoleResult = newUser.AddRole(roleDomain);
        if (addRoleResult.IsFailure)
        {
            return addRoleResult.Errors;
        }

        var idAuthAction = new MasterId(AuthActionType.EmailConfirmation.GetId());
        var addActionResult = newUser.AddAction(idAuthAction, currentDateTime, tokenHashingService);
        if (addActionResult.IsFailure)
        {
            return addActionResult.Errors;
        }

        newUser.RaiseDomainEvents(new UserRegisteredDomainEvent(newUser.Id));

        return newUser;
    }

    public Result ChangePassword(
        string newPassword,
        IPasswordHashingService passwordHashingService)
    {
        var passwordResult = PasswordVO.CreateFromPlainText(newPassword, passwordHashingService);
        if (passwordResult.IsFailure)
        {
            return passwordResult.Errors;
        }

        PasswordHash = passwordResult.Value;
        IsPasswordChanged = true;
        return Result.Success();
    }

    public ResultT<AuthActionTokenDomain> AddAction(
        MasterId idAuthAction,
        DateTime currentDateTime,
        IAuthActionTokenHashingService tokenHashingService)
    {
        var activeTokens = AuthActionTokens
            .Where(x =>
                x.IdUserAction == idAuthAction &&
                !x.Used &&
                x.ExpiresAt.Value >= currentDateTime)
            .ToList();

        foreach (var activeToken in activeTokens)
        {
            activeToken.Revoke();
        }

        var expiredDate = currentDateTime.AddMinutes(30);

        var userActionTokenDomainResult = AuthActionTokenDomain.Create(
            Id,
            idAuthAction,
            expiredDate,
            tokenHashingService);

        if (userActionTokenDomainResult.IsFailure)
        {
            return userActionTokenDomainResult.Errors;
        }

        AuthActionTokens.Add(userActionTokenDomainResult.Value);

        return userActionTokenDomainResult.Value;
    }

    public Result AddRole(RoleDomain role)
    {
        if (role == null)
        {
            return ResultError.NullValue("Role", "Role cannot be null.");
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
