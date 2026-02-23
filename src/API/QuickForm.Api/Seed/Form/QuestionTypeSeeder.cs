using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class QuestionTypeSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);

        var predefineQuestionType = new Dictionary<QuestionTypeType, (DataTypeType DataType, string Description)>
        {
            [QuestionTypeType.InputTypeText] = (DataTypeType.StringType, "Single-line text input (letters, numbers, symbols)."),
            [QuestionTypeType.InputTypeInteger] = (DataTypeType.IntType, "Whole number input (no decimals). Example: 28."),
            [QuestionTypeType.InputTypeDecimal] = (DataTypeType.DecimalType, "Decimal number input (allows fractional values). Example: 28.50."),
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
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
