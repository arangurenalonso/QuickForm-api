using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class FormStatusSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<FormStatusType>()
                                    .Select(enumType => new
                                    {
                                        Id = new MasterId(enumType.GetId()),
                                        KeyName = enumType.GetName(),
                                        Color = enumType.GetColor(),
                                        Icon = enumType.GetIcon()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<FormStatusDomain> existingDomains = await _context.Set<FormStatusDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            MasterId idFormStatus = enumType.Id;
            FormStatusDomain? existingDomain = existingDomains.Find(x => x.Id == idFormStatus);

            if (existingDomain == null)
            {
                FormStatusDomain newDomain = FormStatusDomain.FromExisting(idFormStatus);
                newDomain.Update(
                    enumType.KeyName,
                    enumType.Color,
                    enumType.Icon
                    );
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<FormStatusDomain>().Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    enumType.KeyName,
                    enumType.Color,
                    enumType.Icon
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
