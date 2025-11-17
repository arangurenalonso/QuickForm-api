using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormActionConfiguration : MasterEntityMapBase<FormActionDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<FormActionDomain> builder)
    {
        builder.ToTable("FormAction");
    }
}
