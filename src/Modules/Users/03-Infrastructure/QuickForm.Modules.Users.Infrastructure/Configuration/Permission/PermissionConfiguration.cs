using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
using QuickForm.Common.Infrastructure.Persistence;

namespace QuickForm.Modules.Users.Persistence;
public class PermissionConfiguration : EntityMapBase<PermissionsDomain, PermissionsId>
{
    protected override void Configure(EntityTypeBuilder<PermissionsDomain> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(uat => uat.Id);

        builder.Property(uat => uat.Id)
            .HasConversion(
                new ValueConverter<PermissionsId, Guid>(
                    idVO => idVO.Value,
                    guid => new PermissionsId(guid)
                ))
            .IsRequired();

        builder.Property(uat => uat.IdAction)
            .HasConversion(
                new ValueConverter<PermissionsActionsId, Guid>(
                    idVO => idVO.Value,
                    guid => new PermissionsActionsId(guid)
                ))
            .IsRequired();


        builder.Property(uat => uat.IdResources)
            .HasConversion(
                new ValueConverter<PermissionResourcesId, Guid>(
                    idVo => idVo.Value,
                    guid => new PermissionResourcesId(guid)
                ))
            .IsRequired();


        builder.HasOne(ua => ua.Resources)
            .WithMany(uat => uat.Permissions)
            .HasForeignKey(uat => uat.IdResources)
            .IsRequired();

        builder.HasOne(ua => ua.Action)
            .WithMany(uat => uat.Permissions)
            .HasForeignKey(uat => uat.IdAction)
            .IsRequired();
    }

}
