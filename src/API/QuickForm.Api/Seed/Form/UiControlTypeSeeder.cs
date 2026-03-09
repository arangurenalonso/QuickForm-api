using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class UiControlTypeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<UiControlTypeType>()
                                    .Select(enumType => new
                                    {
                                        Id = new MasterId(enumType.GetId()),
                                        KeyName = enumType.GetName(),
                                        Description = enumType.GetDescription()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<UiControlTypeDomain> existingDomains = await _context.Set<UiControlTypeDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            MasterId id = enumType.Id;
            UiControlTypeDomain? existingDomain = existingDomains.Find(x => x.Id == id);

            if (existingDomain == null)
            {
                UiControlTypeDomain newDomain = UiControlTypeDomain.Create(
                        id,
                        enumType.KeyName,
                        enumType.Description
                        ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<UiControlTypeDomain>().Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName || existingDomain.Description?.Value != enumType.Description)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    enumType.KeyName,
                    enumType.Description
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}

