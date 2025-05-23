﻿using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;
using QuickForm.Modules.Users.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class ResourcesSeeder(UsersDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var enumTypesArray = Enum.GetValues<ResourcesType>()
                                    .Select(enumType => new
                                    {
                                        Id = new ResourcesId(enumType.GetId()),
                                        Description = enumType.GetDetail()
                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<ResourcesDomain> existingDomains = await _context.Resources
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        Guid transactionId = Guid.NewGuid();
        foreach (var enumType in enumTypesArray)
        {
            ResourcesId idVO = enumType.Id;
            ResourcesDomain? existingDomain = existingDomains.Find(x => x.Id == idVO);

            if (existingDomain == null)
            {
                ResourcesDomain newDomain = ResourcesDomain.Create(idVO, enumType.Description).Value;
                newDomain.ClassOrigin = GetType().Name;
                newDomain.TransactionId = transactionId;
                _context.Resources.Add(newDomain);
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
