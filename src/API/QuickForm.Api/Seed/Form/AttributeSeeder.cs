using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

public class AttributeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
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
            })
            .ToList();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<AttributeDomain> existingDomains = await _context.Attribute
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId = Guid.NewGuid();
        foreach (var value in arrayValues)
        {
            AttributeId idEnumType = value.Id;
            AttributeDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                PermissionsDomain newDomain = PermissionsDomain.Create(idEnumType, value.ResourcesId, value.PermissionsActionsId).Value;
                newDomain.ClassOrigin = GetType().Name;
                newDomain.TransactionId = transactionId;
                _context.Attribute.Add(newDomain);
            }
            else if (existingDomain.IdResources != value.ResourcesId || existingDomain.IdAction != value.PermissionsActionsId)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.TransactionId = transactionId;
                existingDomain.Update(value.ResourcesId, value.PermissionsActionsId);
                _context.Attribute.Update(existingDomain);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
