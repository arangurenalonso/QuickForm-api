using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class RuleSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        List<PredefinedRule> predefinedAttributes = PredefinedRules.GetAll();
        var arrayValues = predefinedAttributes
            .Select(rule => new
            {
                Id = new MasterId(rule.RuleType.GetId()),
                KeyName = rule.RuleType.GetName(),
                rule.Description,
                DataTypeId = new DataTypeId(rule.DataTypeType.GetId())
            })
            .ToList();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<RuleDomain> existingDomains = await _context.Set<RuleDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var value in arrayValues)
        {
            MasterId idEnumType = value.Id;
            RuleDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                RuleDomain newDomain = RuleDomain.Create(
                                                    idEnumType,
                                                    value.DataTypeId,
                                                    value.KeyName,
                                                    value.Description
                                                    ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<RuleDomain>().Add(newDomain);
            }
            else if (
                existingDomain.IdDataType != value.DataTypeId ||
                existingDomain.Description?.Value != value.Description ||
                existingDomain.KeyName.Value != value.KeyName 
                )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(value.KeyName, value.Description, value.DataTypeId);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
