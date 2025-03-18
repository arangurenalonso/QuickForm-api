using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

public class DatabaseSeeder(IServiceProvider _serviceProvider, ILogger<DatabaseSeeder> _logger)
{
    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        try
        {
            _logger.LogInformation("Starting Database seeding.");
            // Ejecutar todos los seeders aquí
            var usersDbContext = scopedServices.GetRequiredService<UsersDbContext>();
            var roleSeeder = new RoleSeeder(usersDbContext, scopedServices.GetRequiredService<ILogger<RoleSeeder>>());
            await roleSeeder.SeedAsync();
            var authActionSeeder = new AuthActionSeeder(usersDbContext, scopedServices.GetRequiredService<ILogger<AuthActionSeeder>>());
            await authActionSeeder.SeedAsync();

            // Si tienes más seeders, agrégalos aquí
            // var surveyDbContext = scopedServices.GetRequiredService<SurveyDbContext>()
            // var surveySeeder = new SurveySeeder(surveyDbContext, scopedServices.GetRequiredService<ILogger<SurveySeeder>>())
            // await surveySeeder.SeedAsync()

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
