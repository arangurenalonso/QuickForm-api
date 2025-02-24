namespace QuickForm.Modules.Users.Application;
public interface IUserDapperRepository
{
    Task<UserResponse?> GetUserById(Guid userId);
}
