using System.Data.Common;
using Dapper;
using QuickForm.Common.Infrastructure.Authorization;
using QuickForm.Modules.Users.Application;
namespace QuickForm.Modules.Users.Persistence.Repositories;
public sealed class UserDapperRepository(IDbConnectionFactory dbConnectionFactory) : IUserDapperRepository, IPermissionService
{
    public async Task<UserResponse?> GetUserById(Guid userId)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        const string sql =
           $"""
             SELECT
                 Id AS {nameof(UserResponse.Id)},
                 Email AS {nameof(UserResponse.Email)}
             FROM {Schemas.Auth}.Users
             WHERE Id = @UserId
             AND IsDeleted = 0
        
             """
        ;

        var parameters = new { UserId = userId };

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, parameters);
        return user;
    }
    public async Task<bool> HasPermissionAsync(Guid userId, string resourceKeyName, string actionKeyName)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        var sql = $"""
                SELECT 1
                FROM {Schemas.Auth}.UserRole ur
                JOIN {Schemas.Auth}.Role r
                    ON ur.IdRole = r.Id
                   AND ur.IsDeleted = 0
                   AND r.IsDeleted = 0
                JOIN {Schemas.Auth}.RolePermissions rp
                    ON rp.IdRole = r.Id
                   AND rp.IsDeleted = 0
                JOIN {Schemas.Auth}.Permissions p
                    ON rp.IdPermission = p.Id
                   AND p.IsDeleted = 0
                JOIN {Schemas.Auth}.Resources res
                    ON p.IdResources = res.Id
                   AND res.IsDeleted = 0
                JOIN {Schemas.Auth}.PermissionsActions pa
                    ON p.IdAction = pa.Id
                   AND pa.IsDeleted = 0
                WHERE ur.IdUser = @UserId
                  AND res.KeyName = @ResourceKeyName
                  AND pa.KeyName = @ActionKeyName
                """;

        var result = await connection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new
            {
                UserId = userId,
                ResourceKeyName = resourceKeyName,
                ActionKeyName = actionKeyName
            });

        return result.HasValue;
    }

}
