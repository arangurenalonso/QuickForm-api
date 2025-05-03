using QuickForm.Modules.Survey.Persistence;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class DatabaseSeeder(
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
            var attributeSeeder = new AttributeSeeder(_surveyDbContext, _logger);
            await attributeSeeder.SeedAsync();
            var questionTypeSeeder = new QuestionTypeSeeder(_surveyDbContext, _logger);
            await questionTypeSeeder.SeedAsync();
            var questionTypeAttributesSeeder = new QuestionTypeAttributesSeeder(_surveyDbContext, _logger);
            await questionTypeAttributesSeeder.SeedAsync();



            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
