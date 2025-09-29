using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class QuestionTypeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<QuestionTypeType>()
                                    .Select(enumType => new
                                    {
                                        Id = new QuestionTypeId(enumType.GetId()),
                                        KeyName = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<QuestionTypeDomain> existingDomains = await _context.QuestionType
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            QuestionTypeId idVO = enumType.Id;
            QuestionTypeDomain? existingDomain = existingDomains.Find(x => x.Id == idVO);

            if (existingDomain == null)
            {
                QuestionTypeDomain newDomain = QuestionTypeDomain.Create(idVO, enumType.KeyName).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.QuestionType.Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(enumType.KeyName);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
