using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class QuestionTypeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        }


        var predefineQuestionType = new Dictionary<QuestionTypeType, (DataTypeType DataType, string Description)>
        {
            [QuestionTypeType.InputTypeText] = (DataTypeType.StringType, "Single-line text input (letters, numbers, symbols)."),
            [QuestionTypeType.InputTypeInteger] = (DataTypeType.IntType, "Whole number input (no decimals). Example: 28."),
            [QuestionTypeType.InputTypeDecimal] = (DataTypeType.DecimalType, "Decimal number input (allows fractional values). Example: 28.50."),
            [QuestionTypeType.InputTypeBoolean] = (DataTypeType.BooleanType, "True/False input (checkbox or toggle)."),
            [QuestionTypeType.InputTypeDate] = (DataTypeType.DatetimeType, "Date input (year, month, day). Example: 2024-12-31."),
            [QuestionTypeType.InputTypeDatetime] = (DataTypeType.DatetimeType, "Date and time input (year, month, day, hour, minute). Example: 2024-12-31T23:59."),
            [QuestionTypeType.InputTypeTime] = (DataTypeType.DatetimeType, "Time input (hour, minute). Example: 23:59."),
            [QuestionTypeType.InputTypeSelect] = (DataTypeType.StringType, "Dropdown selection input (predefined options)."),


        };


        var enumTypesArray = predefineQuestionType
                                    .Select(x => new
                                    {
                                        Id = new QuestionTypeId(x.Key.GetId()),
                                        KeyName = x.Key.GetName(),
                                        DescriptionVO = x.Value.Description,
                                        DataTypeId = new DataTypeId(x.Value.DataType.GetId())

                                    })
                                    .ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<QuestionTypeDomain> existingDomains = await _context.QuestionType
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            QuestionTypeId idVO = enumType.Id;
            QuestionTypeDomain? existingDomain = existingDomains.Find(x => x.Id == idVO);

            if (existingDomain == null)
            {
                QuestionTypeDomain newDomain = QuestionTypeDomain.Create(idVO, enumType.KeyName, enumType.DescriptionVO, enumType.DataTypeId).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.QuestionType.Add(newDomain);
            }
            else if (existingDomain.KeyName.Value != enumType.KeyName || existingDomain.Description.Value != enumType.DescriptionVO)
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(enumType.KeyName, enumType.DescriptionVO);
            }
        }

        await _context.SaveChangesAsync();
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
        }
    }
}
