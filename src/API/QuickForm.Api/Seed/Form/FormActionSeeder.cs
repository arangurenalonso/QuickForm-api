using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class FormActionSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<FormActionType>()
                                    .Select(enumType => new
                                    {
                                        Id = new MasterId(enumType.GetId()),
                                        KeyName = enumType.GetName()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<FormActionDomain> existingDomains = await _context.Set<FormActionDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            MasterId idFormAction = enumType.Id;
            FormActionDomain? existingDomain = existingDomains.Find(x => x.Id == idFormAction);

            if (existingDomain == null)
            {
                FormActionDomain newDomain = FormActionDomain.Create(
                        idFormAction,
                        enumType.KeyName
                        ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<FormActionDomain>().Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    enumType.KeyName
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
