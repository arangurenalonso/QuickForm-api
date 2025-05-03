using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class PermissionSeeder(UsersDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        List<PredefinedPermission> predefinedPermissions = PredefinedPermissions.GetAllPredefinedPermissions();
        var arrayValues = predefinedPermissions
            .Select(predefinedPermission => new
            {
                Id =new  PermissionsId(predefinedPermission.Id),
                ResourcesId = new ResourcesId(predefinedPermission.ResourcesType.GetId()),
                PermissionsActionsId = new PermissionsActionsId(predefinedPermission.PermissionsActionType.GetId())
            })
            .ToList();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<PermissionsDomain> existingDomains = await _context.Permissions
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId = Guid.NewGuid();
        foreach (var value in arrayValues)
        {
            PermissionsId idEnumType = value.Id;
            PermissionsDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                PermissionsDomain newDomain = PermissionsDomain.Create(idEnumType, value.ResourcesId,value.PermissionsActionsId).Value;
                newDomain.ClassOrigin = GetType().Name;
                newDomain.TransactionId = transactionId;
                _context.Permissions.Add(newDomain);
            }
            else if (existingDomain.IdResources != value.ResourcesId || existingDomain.IdAction != value.PermissionsActionsId)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.TransactionId = transactionId;
                existingDomain.Update(value.ResourcesId, value.PermissionsActionsId);
                _context.Permissions.Update(existingDomain);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
