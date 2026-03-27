using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class FormRenderSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<FormRenderType>()
                                    .Select(enumType => new
                                    {
                                        Id = new MasterId(enumType.GetId()),
                                        KeyName = enumType.GetName(),
                                        Description = enumType.GetDescription(),
                                        Color = enumType.GetColor(),
                                        Icon = enumType.GetIcon()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<FormRenderDomain> existingDomains = await _context.Set<FormRenderDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            MasterId idFormAction = enumType.Id;
            FormRenderDomain? existingDomain = existingDomains.Find(x => x.Id == idFormAction);

            if (existingDomain == null)
            {
                FormRenderDomain newDomain = FormRenderDomain.CreateWithId(
                        idFormAction,
                        enumType.KeyName,
                        enumType.Description,
                        enumType.Color,
                        enumType.Icon
                        ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<FormRenderDomain>().Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName ||
                existingDomain.Description?.Value != enumType.Description ||
                existingDomain.Color?.Value != enumType.Color ||
                existingDomain.Icon?.Value != enumType.Icon)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    enumType.KeyName,
                    enumType.Description,
                    enumType.Color,
                    enumType.Icon
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
