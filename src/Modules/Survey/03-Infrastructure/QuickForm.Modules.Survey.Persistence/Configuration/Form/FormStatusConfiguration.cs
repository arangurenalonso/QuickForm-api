using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public sealed class FormStatusConfiguration : StatusEntityMapBase<FormStatusDomain>
{
    protected override void ConfigureStatus(EntityTypeBuilder<FormStatusDomain> builder)
    {
        builder.ToTable("FormStatus");
    }
}
