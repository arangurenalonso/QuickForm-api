
namespace QuickForm.Modules.Users.Domain;
public interface IUserRepository
{

    Task<UserDomain?> GetByEmailAsync(string email);
    Task<UserDomain?> GetByIdAsync(UserId userId);
    Task<bool> IsEmailExistsAsync(string email, Guid? idUsuario = null);
    Task<UserDomain?> GetByEmailWithActiveAuthActionsAsync(string email, DateTime currentDatetime);
    Task<UserDomain?> GetByIdWithActiveAuthActionsAsync(UserId userId, DateTime currentDatetime);
    void Insert(UserDomain user);
    void Update(UserDomain user);
}
