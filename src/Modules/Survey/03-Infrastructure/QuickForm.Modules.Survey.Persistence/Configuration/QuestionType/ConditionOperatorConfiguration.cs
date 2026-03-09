using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class ConditionOperatorConfiguration : MasterEntityMapBase<ConditionalOperatorDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<ConditionalOperatorDomain> builder)
    {
        builder.ToTable("ConditionalOperator");

    }
}
