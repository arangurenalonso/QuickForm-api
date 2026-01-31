using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class RuleSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        var predefinedRules = new Dictionary<RuleType, (DataTypeType DataType, string Description, string DefaultValidationMessage, string? RequiredPlaceholder)>
        {
            [RuleType.Min] = (DataTypeType.IntType, "Specifies the minimum allowed numeric value.", "Must be at least {min}", "{min}"),
            [RuleType.Max] = (DataTypeType.IntType, "Specifies the maximum allowed numeric value.", "Cannot exceed {max}","{max}"),
            [RuleType.MinLength] = (DataTypeType.IntType, "Defines the minimum number of characters required.", "Minimum length is {minLength}", "{minLength}"),
            [RuleType.MaxLength] = (DataTypeType.IntType, "Defines the maximum number of characters allowed.", "Maximum length is {maxLength}", "{maxLength}"),
            [RuleType.Required] = (DataTypeType.BooleanType, "Indicates that this field is mandatory and cannot be left empty.", "Field is required",null),
        };

        var arrayValues = (
            from kv in predefinedRules
            let ruleType = kv.Key
            let data = kv.Value
            let id = new MasterId(ruleType.GetId())
            let keyName = ruleType.GetName()
            let description = data.Description
            let dataTypeId = new DataTypeId(data.DataType.GetId())
            let defaultValidationMessage = data.DefaultValidationMessage
            let requiredPlaceholder = data.RequiredPlaceholder
            select new
            {
                Id = id,
                KeyName = keyName,
                Description = description,
                DataTypeId = dataTypeId,
                DefaultValidationMessage = defaultValidationMessage,
                RequiredPlaceholder = requiredPlaceholder
            }
        ).ToList();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<RuleDomain> existingDomains = await _context.Set<RuleDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var value in arrayValues)
        {
            MasterId idEnumType = value.Id;
            RuleDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                RuleDomain newDomain = RuleDomain.Create(
                                                    idEnumType,
                                                    value.DataTypeId,
                                                    value.KeyName,
                                                    value.DefaultValidationMessage,
                                                    value.Description,
                                                    value.RequiredPlaceholder
                                                    ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<RuleDomain>().Add(newDomain);
            }
            else if (
                existingDomain.IdDataType != value.DataTypeId ||
                existingDomain.Description?.Value != value.Description ||
                existingDomain.KeyName.Value != value.KeyName  ||
                existingDomain.DefaultValidationMessageTemplate.ValidationMessage != value.DefaultValidationMessage ||
                existingDomain.DefaultValidationMessageTemplate.PlaceholderKey != value.RequiredPlaceholder
                )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                    value.KeyName,
                    value.Description,
                    value.DataTypeId,
                    value.DefaultValidationMessage,
                    value.RequiredPlaceholder
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
