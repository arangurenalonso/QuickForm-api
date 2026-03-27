using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormRenderConfiguration : MasterEntityMapBase<FormRenderDomain>
{
    protected override void ConfigureMaster(EntityTypeBuilder<FormRenderDomain> builder)
    {
        builder.ToTable("FormRender");


        builder.OwnsOne(e => e.Color, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Color")
                 .HasMaxLength(50)
                 .IsRequired(false);
        });

        builder.OwnsOne(e => e.Icon, owned =>
        {
            owned.Property(v => v.Value)
                 .HasColumnName("Icon")
                 .HasMaxLength(100)
                 .IsRequired(false);
        });

    }
}
