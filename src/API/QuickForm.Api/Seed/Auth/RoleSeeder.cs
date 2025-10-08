using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class RoleSeeder(UsersDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<RoleType>()
                                    .Select(enumType => new
                                    {
                                        Id = new MasterId(enumType.GetId()),
                                        KeyName = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<RoleDomain> existingDomains = await _context.Role
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            MasterId idRole = enumType.Id;
            RoleDomain? existingDomain = existingDomains.Find(x=>x.Id==idRole);

            if (existingDomain == null)
            {
                RoleDomain newDomain = RoleDomain.Create(idRole,enumType.KeyName).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Role.Add(newDomain);
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
