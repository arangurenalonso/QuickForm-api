using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class ResourcesConfiguration : EntityMapBase<ResourcesDomain, ResourcesId>
{
    protected override void Configure(EntityTypeBuilder<ResourcesDomain> builder)
    {
        builder.ToTable("Resources");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<ResourcesId, Guid>(
                    idVO => idVO.Value,
                    guid => new ResourcesId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => PermissionResourcesDescription.Create(descriptionString).Value
                    );

    }
}
