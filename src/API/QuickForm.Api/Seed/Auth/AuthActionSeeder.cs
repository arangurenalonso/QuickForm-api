using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class AuthActionSeeder(UsersDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);

        var enumTypesArray = Enum.GetValues<AuthActionType>() 
                                    .Select(enumType => new
                                    {
                                        Id = new AuthActionId(enumType.GetId()),
                                        Description = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<AuthActionDomain> existingDomains = await _context.AuthAction
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId = Guid.NewGuid();
        foreach (var enumType in enumTypesArray)
        {
            AuthActionId idEnumType = enumType.Id;
            AuthActionDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                AuthActionDomain newDomain = AuthActionDomain.Create(idEnumType, enumType.Description).Value;
                newDomain.ClassOrigin = GetType().Name;
                newDomain.TransactionId = transactionId;
                _context.AuthAction.Add(newDomain);
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
