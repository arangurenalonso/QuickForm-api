using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

public class RoleSeeder(UsersDbContext _context, ILogger<RoleSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting RoleType seeding...");
        var roleTypeArray = Enum.GetValues(typeof(RoleType))
            .Cast<RoleType>()
            .Select(role => new
            {
                Id = new RoleId(role.GetId()),
                Description = role.GetDetail()
            })
            .ToList();

        var roleIds = roleTypeArray.Select(x => x.Id).ToList();

        List<RoleDomain> existingRoles = await _context.Role
                                            .Where(x => roleIds.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId= Guid.NewGuid();
        foreach (var roleType in roleTypeArray)
        {
            RoleId idRole = roleType.Id;
            RoleDomain? existingRole = existingRoles.Find(x=>x.Id==idRole);

            if (existingRole == null)
            {
                RoleDomain newRole = RoleDomain.Create(idRole,roleType.Description).Value;
                newRole.ClassOrigin = "RoleSeeder";
                newRole.TransactionId = transactionId;
                _context.Role.Add(newRole);
            }
            else if (existingRole.Description.Value != roleType.Description)
            {
                existingRole.ClassOrigin = "RoleSeeder";
                existingRole.TransactionId = transactionId;
                existingRole.Update(roleType.Description);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("RoleType seeding completed.");
    }
}
