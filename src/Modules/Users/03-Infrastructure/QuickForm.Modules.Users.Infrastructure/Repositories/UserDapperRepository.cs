using System.Data.Common;
using Dapper;
using QuickForm.Modules.Users.Application;
namespace QuickForm.Modules.Users.Persistence.Repositories;
public sealed class UserDapperRepository(IDbConnectionFactory dbConnectionFactory) : IUserDapperRepository
{
    public async Task<UserResponse?> GetUserById(Guid userId)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        const string sql =
           $"""
             SELECT
                 Id AS {nameof(UserResponse.Id)},
                 Email AS {nameof(UserResponse.Email)},
                 Name AS {nameof(UserResponse.FirstName)},
                 LastName AS {nameof(UserResponse.LastName)}
             FROM {Schemas.Auth}.Users
             WHERE Id = @UserId
        
             """
        ;

        var parameters = new { UserId = userId };

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, parameters);
        return user;
    }
}
