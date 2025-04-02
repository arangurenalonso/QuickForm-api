using QuickForm.Modules.Survey.Persistence;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

public class DatabaseSeeder(
        UsersDbContext _userContext, 
        SurveyDbContext _surveyDbContext, 
        ILogger<DatabaseSeeder> _logger
    )
{
    public async Task SeedAsync()
    {

        try
        {
            _logger.LogInformation("Starting Database seeding.");
            var roleSeeder = new RoleSeeder(_userContext, _logger);
            await roleSeeder.SeedAsync();
            var authActionSeeder = new AuthActionSeeder(_userContext, _logger);
            await authActionSeeder.SeedAsync();
            var permissionsActionSeeder = new PermissionsActionSeeder(_userContext, _logger);
            await permissionsActionSeeder.SeedAsync();
            var resourcesSeeder = new ResourcesSeeder(_userContext, _logger);
            await resourcesSeeder.SeedAsync();
            var permissionSeeder = new PermissionSeeder(_userContext, _logger);
            await permissionSeeder.SeedAsync();

            var datatypeSeeder = new DatatypeSeeder(_surveyDbContext, _logger);
            await datatypeSeeder.SeedAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
