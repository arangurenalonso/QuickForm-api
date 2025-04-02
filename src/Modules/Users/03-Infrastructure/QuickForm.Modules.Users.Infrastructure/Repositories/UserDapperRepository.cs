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
    public async Task<bool> HasPermissionAsync(Guid userId, string resourceDescription, string actionDescription)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        var sql =
           $"""
            SELECT 
                1 
            FROM 
            {Schemas.Auth}.UserRole ur
                JOIN {Schemas.Auth}.Role r 
                    ON ur.IdRole = r.Id AND ur.IsActive = 1 AND r.IsActive = 1
                JOIN {Schemas.Auth}.RolePermissions rp 
                    ON rp.IdRole = r.Id AND rp.IsActive = 1 
                JOIN {Schemas.Auth}.Permissions p 
                    ON rp.IdPermission = p.Id AND p.IsActive = 1
                JOIN {Schemas.Auth}.Resources res 
                    ON p.IdResources = res.Id AND res.IsActive = 1
                JOIN {Schemas.Auth}.PermissionsActions pa 
                    ON p.IdAction = pa.Id AND pa.IsActive = 1
            WHERE 
                ur.idUser = @UserId 
                AND ur.IsActive = 1
                AND res.description = @ResourceDescription 
                AND pa.description = @ActionDescription
            """;

        var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId, ResourceDescription = resourceDescription, ActionDescription = actionDescription });

        return result.HasValue;
    }

}
