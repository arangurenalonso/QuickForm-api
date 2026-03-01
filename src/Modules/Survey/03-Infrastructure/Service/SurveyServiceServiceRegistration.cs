using Microsoft.Extensions.DependencyInjection;
using QuickForm.Modules.Survey.Application;

namespace QuickForm.Modules.Survey.Service;
public static class SurveyServiceServiceRegistration
{
    public static IServiceCollection AddSurveyServiceServices(this IServiceCollection services)
    {

        services.AddScoped<IExcelService, ExcelService>();

        return services;
    }

}
