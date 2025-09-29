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
                                        Id = new RoleId(enumType.GetId()),
                                        Description = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<RoleDomain> existingDomains = await _context.Role
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            RoleId idRole = enumType.Id;
            RoleDomain? existingDomain = existingDomains.Find(x=>x.Id==idRole);

            if (existingDomain == null)
            {
                RoleDomain newDomain = RoleDomain.Create(idRole,enumType.Description).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Role.Add(newDomain);
            }
            else if (existingDomain.Description.Value != enumType.Description)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(enumType.Description);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
