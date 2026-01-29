using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;
using QuickForm.Modules.Survey.Persistence;

namespace QuickForm.Api.Seed;

internal sealed class QuestionTypeRuleSeeder(SurveyDbContext _context, ILogger<DatabaseSeeder> _logger)
{
    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting {SeederName} seeding...", GetType().Name);
        var predefinedQuestionTypeRule = new Dictionary<QuestionTypeType, (Guid IdRelation, RuleType Rule,bool IsRequired, string DefaultValidationMessage)[]>
        {
            [QuestionTypeType.InputTypeText] = new[]
            {
                (new Guid("737648B7-E448-4504-973D-DDE7556A3FB0"), RuleType.MinLength,false, "Minimum length is {min}"),
                (new Guid("ADBD7C1A-053E-4A17-B1C2-71B84FD31CD1"), RuleType.MaxLength,false,"Maximum length is {max}"),
                (new Guid("B9260120-23E2-472F-BCA4-32DCCEE85D32"), RuleType.Required,false,"Field is required")
            },
            [QuestionTypeType.InputTypeNumber] = new[]
            {
                (new Guid("F5AB345C-49AC-43DF-A6C8-16549BCE6A5D"), RuleType.Min,false,"Must be at least {min}"),
                (new Guid("CBEF1C4F-FA03-46B3-B52E-6542489FD708"), RuleType.Max,false,"Cannot exceed {max}"),
                (new Guid("75C7AE09-96E5-4C1E-A9D2-BDC1A1767ADF"), RuleType.Required,false,"Field is required"),
            },
        };

        var enumTypesArray = (
            from questionTypeEntry in predefinedQuestionTypeRule
            from rule in questionTypeEntry.Value
            let idQuestionType = new QuestionTypeId(questionTypeEntry.Key.GetId())
            let idRule = new MasterId(rule.Rule.GetId())
            let idRelation = new QuestionTypeRuleId(rule.IdRelation)
            let isRequired = rule.IsRequired    
            let defaultValidationMessage = rule.DefaultValidationMessage
            select new
            {
                Id = idRelation,
                IdQuestionType = idQuestionType,
                IdRule = idRule,
                IsRequired = isRequired,
                DefaultValidationMessage = defaultValidationMessage
            }).ToList();

        var ids = enumTypesArray.Select(x => x.Id).ToList();

        List<QuestionTypeRuleDomain> existingDomains = await _context.Set<QuestionTypeRuleDomain>()
                                            .Where(x => ids.Contains(x.Id))
                                            .ToListAsync();
        foreach (var enumType in enumTypesArray)
        {
            QuestionTypeRuleId idQuestionTypeRule = enumType.Id;
            QuestionTypeRuleDomain? existingDomain = existingDomains.Find(x => x.Id == idQuestionTypeRule);

            if (existingDomain == null)
            {
                QuestionTypeRuleDomain newDomain = QuestionTypeRuleDomain.Create(
                        idQuestionTypeRule,
                        enumType.IdQuestionType,
                        enumType.IdRule,
                        enumType.IsRequired,
                        enumType.DefaultValidationMessage
                        ).Value;
                newDomain.ClassOrigin = GetType().Name;
                _context.Set<QuestionTypeRuleDomain>().Add(newDomain);
            }
            else if (existingDomain.IdQuestionType != enumType.IdQuestionType ||
                     existingDomain.IdRule != enumType.IdRule ||
                     existingDomain.IsRequired != enumType.IsRequired ||
                     existingDomain.DefaultValidationMessage != enumType.DefaultValidationMessage

                     )
            {
                existingDomain.ClassOrigin = GetType().Name;
                existingDomain.Update(
                        enumType.IdQuestionType,
                        enumType.IdRule,
                        enumType.IsRequired,
                        enumType.DefaultValidationMessage
                    );
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("{SeederName} seeding completed", GetType().Name);
    }
}
