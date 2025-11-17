using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class FormStatusPermissionSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{
    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var permissionsByStatus = new Dictionary<FormStatusType, (Guid PermissionId, FormActionType Action)[]>
        {
            [FormStatusType.Draft] = new[]
            {
                (new Guid("76886195-D738-41E0-91B6-613BB8000AF8"), FormActionType.FormEdit),
                (new Guid("148D5AA9-351A-4224-AE53-74FD494F63F1"), FormActionType.FormPublish)
            },
            [FormStatusType.Published] = new[]
            {
                (new Guid("913C25B9-5081-487C-A520-12802B18A885"), FormActionType.FormPause),
                (new Guid("A61BA8BA-A5A8-41F8-AB6C-7697A55D9C85"), FormActionType.FormClose),
                (new Guid("9CB5FD57-BECE-4426-A946-F7FA79553F32"), FormActionType.ViewResponses)
            },
            [FormStatusType.Paused] = new[]
            {
                (new Guid("1E1D2725-BFD4-4D1B-9326-F36E945020D2"), FormActionType.FormEdit),
                (new Guid("DDA963FF-07B7-4D1B-BDB2-9CFB4693CFA8"), FormActionType.FormResume),
                (new Guid("545EB2CB-9E2E-4CBE-84D9-12572125C8FD"), FormActionType.FormClose),
                (new Guid("8E2025E1-4A0B-4AA1-858F-E50E50674566"), FormActionType.ViewResponses)
            },
            [FormStatusType.Closed] = new[]
            {
                (new Guid("46C570F7-1B42-47E1-9637-5D0355F13D1B"), FormActionType.ViewResponses)
            }
        };

        var enumTypesArray = (
            from statusEntry in permissionsByStatus
            from action in statusEntry.Value
            let idStatus = new MasterId(statusEntry.Key.GetId())
            let idAction = new MasterId(action.Action.GetId())
            let idPermission = new FormStatusPermissionId(action.PermissionId)
            select new
            {
                Id = idPermission,
                IdFormStatus = idStatus,
                IdFormAction = idAction
            }).ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<FormStatusPermissionDomain> existingDomains = await _context.Set<FormStatusPermissionDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            FormStatusPermissionId idFormStatusPermission = enumType.Id;
            FormStatusPermissionDomain? existingDomain = existingDomains.Find(x => x.Id == idFormStatusPermission);

            if (existingDomain == null)
            {
                FormStatusPermissionDomain newDomain = FormStatusPermissionDomain.Create(
                        idFormStatusPermission,
                        enumType.IdFormAction,
                        enumType.IdFormStatus
                        ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<FormStatusPermissionDomain>().Add(newDomain);
            }
            else if (existingDomain.IdFormStatus != enumType.IdFormStatus ||
                     existingDomain.IdFormAction !=  enumType.IdFormAction)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    enumType.IdFormAction,
                    enumType.IdFormStatus
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
