using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal class DatatypeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<DataTypeType>()
                                    .Select(enumType => new
                                    {
                                        Id = new DataTypeId(enumType.GetId()),
                                        Description = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<DataTypeDomain> existingDomains = await _context.DataType
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId = Guid.NewGuid();
        foreach (var enumType in enumTypesArray)
        {
            DataTypeId idVO = enumType.Id;
            DataTypeDomain? existingDomain = existingDomains.Find(x => x.Id == idVO);

            if (existingDomain == null)
            {
                DataTypeDomain newDomain = DataTypeDomain.Create(idVO, enumType.Description).Value;
                newDomain.ClassOrigin = GetType().Name;
                newDomain.TransactionId = transactionId;
                _context.DataType.Add(newDomain);
            }
            else if (existingDomain.Description.Value != enumType.Description)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.TransactionId = transactionId;
                existingDomain.Update(enumType.Description);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
