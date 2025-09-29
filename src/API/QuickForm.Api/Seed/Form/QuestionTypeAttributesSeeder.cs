using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed  class QuestionTypeAttributesSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        List<QuestionTypeAttributeSeedConfig> predefinedValues = PredefinedQuestionTypeAttributes.GetAll();
        var arrayValues = predefinedValues
                            .SelectMany(q => q.AttributeQuestionTypeAttribute.Select(attr => new
                            {
                                Id= new QuestionTypeAttributeId( attr.IdRelation),
                                IdQuestionType=new QuestionTypeId(q.QuestionTypeType.GetId()),
                                IdAttribute = new AttributeId(attr.AttributeType.GetId()),
                                attr.IsRequired
                            }))
                            .ToArray();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<QuestionTypeAttributeDomain> existingDomains = await _context.QuestionTypeAttribute
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var value in arrayValues)
        {
            QuestionTypeAttributeId idEnumType = value.Id;
            QuestionTypeAttributeDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                QuestionTypeAttributeDomain newDomain = QuestionTypeAttributeDomain.Create(
                                                    idEnumType,
                                                    value.IdQuestionType,
                                                    value.IdAttribute,
                                                    value.IsRequired
                                                    ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.QuestionTypeAttribute.Add(newDomain);
            }
            else if (
                existingDomain.IdQuestionType != value.IdQuestionType ||
                existingDomain.IdAttribute != value.IdAttribute ||
                existingDomain.IsRequired != value.IsRequired
                )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(value.IdQuestionType, value.IdAttribute, value.IsRequired);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
