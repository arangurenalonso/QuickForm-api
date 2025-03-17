using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class PermissionResourcesConfiguration : EntityMapBase<PermissionResourcesDomain, PermissionResourcesId>
{
    protected override void Configure(EntityTypeBuilder<PermissionResourcesDomain> builder)
    {
        builder.ToTable("PermissionResources");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<PermissionResourcesId, Guid>(
                    idVO => idVO.Value,
                    guid => new PermissionResourcesId(guid)
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
