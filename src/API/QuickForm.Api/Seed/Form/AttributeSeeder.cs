using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class AttributeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        List<PredefinedAttribute> predefinedAttributes = PredefinedAttributes.GetAll();
        var arrayValues = predefinedAttributes
            .Select(attribute => new
            {
                Id = new AttributeId(attribute.AttributeType.GetId()),
                KeyName = attribute.AttributeType.GetDetail(),
                attribute.Description,
                DataTypeId = new DataTypeId(attribute.DataTypeType.GetId()),
                attribute.MustBeUnique
            })
            .ToList();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<AttributeDomain> existingDomains = await _context.Attribute
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var value in arrayValues)
        {
            AttributeId idEnumType = value.Id;
            AttributeDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                AttributeDomain newDomain = AttributeDomain.Create(
                                                    idEnumType, 
                                                    value.KeyName, 
                                                    value.Description,
                                                    value.DataTypeId,
                                                    value.MustBeUnique
                                                    ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Attribute.Add(newDomain);
            }
            else if (
                existingDomain.IdDataType != value.DataTypeId || 
                existingDomain.Description.Value != value.Description||
                existingDomain.KeyName.Value != value.KeyName ||
                existingDomain.MustBeUnique != value.MustBeUnique
                )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(value.KeyName, value.Description,value.DataTypeId, value.MustBeUnique);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
