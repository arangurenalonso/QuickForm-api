
namespace QuickForm.Modules.Users.Domain;
public interface IUserRepository
{

    Task<UserDomain?> GetByEmailAsync(EmailVO email);
    Task<UserDomain?> GetByIdAsync(UserId userId);
    Task<bool> IsEmailExistsAsync(EmailVO email, UserId? userId = null);
    Task<UserDomain?> GetByEmailWithActiveAuthActionsAsync(EmailVO email, DateTime currentDatetime);
    Task<UserDomain?> GetByIdWithActiveAuthActionsAsync(UserId userId, DateTime currentDatetime);
    void Insert(UserDomain user);
    void Update(UserDomain user);
}
