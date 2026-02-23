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


        var predefinedValues = new Dictionary<QuestionTypeType, (Guid QuestionTypeAttributeId, AttributeType AttributeType,bool IsRequired)[]>
        {
            [QuestionTypeType.InputTypeText] = new[]
            {
                (Guid.Parse("2194295A-C550-49D9-9CE4-541347C2F21A"), AttributeType.Name, true),
                (Guid.Parse("2C1487CF-921D-42DA-9734-93D7076C534C"), AttributeType.Label, true),
                (Guid.Parse("E2F0D7CE-26E5-4881-8A23-E30BCA696908"), AttributeType.HelperText, false),
                (Guid.Parse("C98EB09A-AD81-46D3-A0EB-F14395B30DC3"), AttributeType.Placeholder, false),
                (Guid.Parse("C8AD7891-6FD7-42B6-8B7E-A9AF92692313"), AttributeType.InformationText, false),
            },
            [QuestionTypeType.InputTypeInteger] = new[]
                {
                (Guid.Parse("31F8B98F-90EA-45B5-8AF0-AE7A126B5603"), AttributeType.Name, true),
                (Guid.Parse("54AB1A43-0E16-49BA-8DED-19AE0BC7ACB7"), AttributeType.Label, true),
                (Guid.Parse("A02BD294-0FBD-4C6E-955E-BF7710877EAD"), AttributeType.HelperText, false),
                (Guid.Parse("564466CA-0E44-4DE1-94E5-98AB39B0D1B8"), AttributeType.Placeholder, false),
                (Guid.Parse("F7CB2D78-D0B8-4A28-960D-05C88BB10A56"), AttributeType.InformationText, false),
                (Guid.Parse("DA574AF2-DF7A-4ABF-87D9-6424BBA5ED5C"), AttributeType.Prefix, false),
                (Guid.Parse("BBDBDCB3-6BDE-4F5B-8A8D-16F629BE2A88"), AttributeType.Suffix, false),
                (Guid.Parse("F1A91279-CF55-4889-826F-2F9123AC0E4A"), AttributeType.AllowNegative, false),
            },
            [QuestionTypeType.InputTypeDecimal] = new[]
            {
                (Guid.Parse("917D8C21-A6A6-44CB-8573-5EC957EFA90A"), AttributeType.Name, true),
                (Guid.Parse("94AB94DF-1323-4CD4-B366-8C459AE906A0"), AttributeType.Label, true),
                (Guid.Parse("7C93FE32-2E07-4A94-8ECC-3AD4D7A906C3"), AttributeType.HelperText, false),
                (Guid.Parse("BFB07569-D843-4144-925C-1F37EE17F45E"), AttributeType.Placeholder, false),
                (Guid.Parse("736A723E-33C7-46B3-A45A-E752DD7FE30D"), AttributeType.InformationText, false),
                (Guid.Parse("F27B023C-F469-442A-A736-A3EA82BC1F60"), AttributeType.Prefix, false),
                (Guid.Parse("44E42CD8-619D-48B6-B91D-DE406D58A5A7"), AttributeType.Suffix, false),
                (Guid.Parse("BD1CC1AF-3420-4AED-854C-26209D2E68E4"), AttributeType.DecimalScale, false),
                (Guid.Parse("3D5C5249-BD49-4852-9A5C-79120C503E10"), AttributeType.AllowNegative, false),
            },
        };


        var arrayValues = predefinedValues
                            .SelectMany(q => q.Value.Select(attr => new
                            {
                                Id= new QuestionTypeAttributeId( attr.QuestionTypeAttributeId),
                                IdQuestionType=new QuestionTypeId(q.Key.GetId()),
                                IdAttribute = new MasterId(attr.AttributeType.GetId()),
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
