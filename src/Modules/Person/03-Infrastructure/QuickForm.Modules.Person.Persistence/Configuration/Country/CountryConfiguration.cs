
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Person.Domain;

namespace QuickForm.Modules.Person.Persistence;
public sealed class CountryConfiguration : MasterEntityMapBase<CountryDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<CountryDomain> builder)
    {
        builder.ToTable("Country", "Master");

    }
}

