using System.Reflection;
using QuickForm.Api;
using QuickForm.Api.Seed;
using QuickForm.Common.Application;
using QuickForm.Common.Infrastructure;
using QuickForm.Common.Presentation;
using QuickForm.Modules.Survey.Host;
using QuickForm.Modules.Users.Host;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DatabaseSeeder>(); 


Assembly[] moduleApplicationAssemblies = [
    QuickForm.Modules.Users.Application.AssemblyReference.Assembly,
    QuickForm.Modules.Survey.Application.AssemblyReference.Assembly,
    ];

builder.Configuration.AddModuleConfiguration(["users", "survey", "common"]);

string environment = builder.Configuration["Common:environment"] ?? "";

builder.Services.AddCommonServicesServices(
    [
        SurveyModule.ConfigureConsumers
    ], builder.Configuration, environment);
builder.Services.AddApplication(moduleApplicationAssemblies);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddSurveyModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.UseExceptionHandler();

app.UseAuthentication();    
app.UseAuthorization();

app.MapPost("/api/admin/seed", async (DatabaseSeeder seeder, ILogger<DatabaseSeeder> logger) =>
{
    try
    {
        await seeder.SeedAsync();
        return Results.Ok("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while executing the seeder.");
        return Results.Problem("Failed to execute database seeder.");
    }
});

await app.RunAsync();


// Abrir la consola de NuGet
// Seleccionar el Api
// Seleccionar la carpeta Infrastructure
// Add-Migration IndexIsActive -Context UsersDbContext -OutputDir Migrations
// Add-Migration AddEntitiesQuestionAttributeValueAndQuestion -Context SurveyDbContext -OutputDir Migrations

// //<auto-generated/>
