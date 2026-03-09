using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class QuestionTypeFilterSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);


        var predefinedValues = new Dictionary<QuestionTypeType, (Guid QuestionTypeFilterId, ConditionalOperatorType ConditionalOperator, UiControlTypeType UiControlType)[]>
        {
            [QuestionTypeType.InputTypeText] = new[]
            {
                (Guid.Parse("B57584F0-3714-4E4D-9C1B-D479921DA3D1"), ConditionalOperatorType.Contains, UiControlTypeType.Text),
                (Guid.Parse("C875B026-B079-417F-9077-3821FEEE25E2"), ConditionalOperatorType.NotContains, UiControlTypeType.Text),
                (Guid.Parse("069BBA21-2D79-4E19-8F8D-805471BED3F4"), ConditionalOperatorType.Equals, UiControlTypeType.Text),
                (Guid.Parse("4EE2CA67-34F2-4D08-A11E-C3799EB30100"), ConditionalOperatorType.NotEquals, UiControlTypeType.Text),
                (Guid.Parse("EFB02A81-4637-43CB-8D2D-07FF4B79D243"), ConditionalOperatorType.StartsWith, UiControlTypeType.Text),
                (Guid.Parse("F4EB9776-C0E4-4A56-A798-8E7D327BC78D"), ConditionalOperatorType.EndsWith, UiControlTypeType.Text),
                (Guid.Parse("1F336303-14A7-48E0-BD0A-5FF28DD9D0E3"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("ED8B763D-DEB3-46DB-A8DC-BEFAE6480A55"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
            [QuestionTypeType.InputTypeInteger] = new[]
            {
                (Guid.Parse("78341E4A-5C74-446C-8AA0-58901CC25A79"), ConditionalOperatorType.Equals, UiControlTypeType.Number),
                (Guid.Parse("7827F1BD-E10B-49A6-BCCB-05D1A2F5DBDD"), ConditionalOperatorType.NotEquals, UiControlTypeType.Number),
                (Guid.Parse("DFD722B8-1C5D-4D82-8139-915FE12F0B02"), ConditionalOperatorType.GreaterThan, UiControlTypeType.Number),
                (Guid.Parse("0154D6E4-0699-4674-A51B-2CACFEC333B3"), ConditionalOperatorType.GreaterThanOrEqual, UiControlTypeType.Number),
                (Guid.Parse("B2AD491C-3781-47B2-BB10-944F9055A5DA"), ConditionalOperatorType.LessThan, UiControlTypeType.Number),
                (Guid.Parse("E4812559-A0F2-4B25-A721-C41D0562F842"), ConditionalOperatorType.LessThanOrEqual, UiControlTypeType.Number),
                (Guid.Parse("1C4A8B46-4BEF-41D8-96C0-D21241D0D5CD"), ConditionalOperatorType.Between, UiControlTypeType.RangeNumber),
                (Guid.Parse("221DADEB-9D0E-4539-BB78-7B1A4E888491"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("A99DB3B3-C36C-46F4-944C-9F9E73B08EA6"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
            [QuestionTypeType.InputTypeDecimal] = new[]
            {
                (Guid.Parse("BB4E8106-AD92-4B2B-808D-3CD254880803"), ConditionalOperatorType.Equals, UiControlTypeType.Number),
                (Guid.Parse("77B2403B-67C9-4AAF-BBC9-2E17CBEE50BF"), ConditionalOperatorType.NotEquals, UiControlTypeType.Number),
                (Guid.Parse("3059AA17-8B96-44C0-BEB4-904E64E1255B"), ConditionalOperatorType.GreaterThan, UiControlTypeType.Number),
                (Guid.Parse("E900E3A3-1BFB-49A9-9ED2-1E63236B4FF1"), ConditionalOperatorType.GreaterThanOrEqual, UiControlTypeType.Number),
                (Guid.Parse("2875C116-1A2C-4130-A4ED-545238D5546F"), ConditionalOperatorType.LessThan, UiControlTypeType.Number),
                (Guid.Parse("AB6B12DB-8F12-4396-B8CA-B5A6B363487C"), ConditionalOperatorType.LessThanOrEqual, UiControlTypeType.Number),
                (Guid.Parse("C80F2FAB-3BB5-4DAC-A71F-E2D6E6960941"), ConditionalOperatorType.Between, UiControlTypeType.RangeNumber),
                (Guid.Parse("410A7139-E710-4150-A935-58B9EEC9BDEE"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("1A26D99D-3FF5-4B71-9573-A1E9A8B6069E"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
            [QuestionTypeType.InputTypeBoolean] = new[]
            {
                (Guid.Parse("46E1C63D-970E-43F2-99A4-4BBE304F937B"), ConditionalOperatorType.IsTrue, UiControlTypeType.None),
                (Guid.Parse("13BE027A-028E-4C64-A5D9-0A9F8F0C3CEE"), ConditionalOperatorType.IsFalse, UiControlTypeType.None),
                (Guid.Parse("98513024-2C27-4F35-A730-CD830B871C89"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("00481D78-C5E8-49AF-9E5E-C901306FE5CD"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },

            [QuestionTypeType.InputTypeDate] = new[]
            {
                (Guid.Parse("4A09DBEB-6F25-42F2-AA7B-60F3727B4068"), ConditionalOperatorType.On, UiControlTypeType.Date),
                (Guid.Parse("2E5BD42D-81A4-4DB8-88A8-C69F8875AF9B"), ConditionalOperatorType.Before, UiControlTypeType.Date),
                (Guid.Parse("407D7D0B-1813-4DA6-8ED9-6870088BA97D"), ConditionalOperatorType.After, UiControlTypeType.Date),
                (Guid.Parse("E8A70964-A6AB-46AE-8C8E-221E96044647"), ConditionalOperatorType.OnOrBefore, UiControlTypeType.Date),
                (Guid.Parse("855F6DF1-20EC-4BDD-B4F1-2D59181831B3"), ConditionalOperatorType.OnOrAfter, UiControlTypeType.Date),
                (Guid.Parse("64339710-0ABA-44B9-8E0D-B5092418246D"), ConditionalOperatorType.Between, UiControlTypeType.RangeDate),
                (Guid.Parse("7996F2E8-B454-4260-B461-950C39BAA9D4"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("E85C2299-67B1-46D2-B97F-A656A3A7C348"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
            [QuestionTypeType.InputTypeTime] = new[]
            {
                (Guid.Parse("45F6C644-0F21-49FF-A0A2-A9712712221B"), ConditionalOperatorType.Equals, UiControlTypeType.Time),
                (Guid.Parse("ECFD1E4E-F014-4F80-8244-F07AE0D0F300"), ConditionalOperatorType.Before, UiControlTypeType.Time),
                (Guid.Parse("BC6954CA-B1DE-466F-984E-4701A98D5DE9"), ConditionalOperatorType.After, UiControlTypeType.Time),
                (Guid.Parse("5DCE0554-78CD-4654-A640-3BD5C066B89F"), ConditionalOperatorType.Between, UiControlTypeType.RangeTime),
                (Guid.Parse("A3D4DF62-6742-4262-BEA5-9396079B41A8"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("1AFA1C08-EF09-4DAD-AA49-789E761E6820"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
            [QuestionTypeType.InputTypeDatetime] = new[]
            {
                (Guid.Parse("DA9703AA-7AC3-41E8-BB65-54DB2964307A"), ConditionalOperatorType.On, UiControlTypeType.Datetime),
                (Guid.Parse("CEDE4FE5-A87C-40B5-9B91-11BD10AEBC0F"), ConditionalOperatorType.Before, UiControlTypeType.Datetime),
                (Guid.Parse("926EB679-9CCD-4C25-B5B4-06B5032E3C72"), ConditionalOperatorType.After, UiControlTypeType.Datetime),
                (Guid.Parse("DE2CA1B3-D3FC-47A3-BD05-F010EE688F92"), ConditionalOperatorType.OnOrBefore, UiControlTypeType.Datetime),
                (Guid.Parse("14161AA3-504A-4947-BA2C-CB91B9DA569C"), ConditionalOperatorType.OnOrAfter, UiControlTypeType.Datetime),
                (Guid.Parse("1EC0E308-A481-4A8A-BAB8-88D0C21563A7"), ConditionalOperatorType.Between, UiControlTypeType.RangeDatetime),
                (Guid.Parse("FA33B860-EBBD-4363-BE32-D74B5471B95E"), ConditionalOperatorType.IsEmpty, UiControlTypeType.None),
                (Guid.Parse("35B582EF-73C6-4C91-AE36-3869A30FC5B9"), ConditionalOperatorType.IsNotEmpty, UiControlTypeType.None),
            },
        };


        var arrayValues = predefinedValues
                            .SelectMany(q => q.Value.Select(attr => new
                            {
                                Id = new QuestionTypeFilterId(attr.QuestionTypeFilterId),
                                IdQuestionType = new QuestionTypeId(q.Key.GetId()),
                                IdConditionalOperator = new MasterId(attr.ConditionalOperator.GetId()),
                                IdUiControlType = new MasterId(attr.UiControlType.GetId()),
                            }))
                            .ToArray();



        var ids = arrayValues.Select(x => x.Id).ToList();

        List<QuestionTypeFilterDomain> existingDomains = await _context.Set<QuestionTypeFilterDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var value in arrayValues)
        {
            QuestionTypeFilterId idEnumType = value.Id;
            QuestionTypeFilterDomain? existingDomain = existingDomains.Find(x => x.Id == idEnumType);

            if (existingDomain == null)
            {
                QuestionTypeFilterDomain newDomain = QuestionTypeFilterDomain.Create(
                                                    idEnumType,
                                                    value.IdConditionalOperator,
                                                    value.IdUiControlType,
                                                    value.IdQuestionType
                                                    ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<QuestionTypeFilterDomain>().Add(newDomain);
            }
            else if (
                existingDomain.IdQuestionType != value.IdQuestionType ||
                existingDomain.IdConditionalOperator != value.IdConditionalOperator ||
                existingDomain.IdUiControlType != value.IdUiControlType
                )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(value.IdConditionalOperator, value.IdUiControlType, value.IdQuestionType);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
