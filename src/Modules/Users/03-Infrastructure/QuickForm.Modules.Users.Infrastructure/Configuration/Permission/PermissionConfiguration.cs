using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

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
                new ValueConverter<MasterId, Guid>(
                    idVO => idVO.Value,
                    guid => new MasterId(guid)
                ))
            .IsRequired();


        builder.Property(uat => uat.IdResources)
            .HasConversion(
                new ValueConverter<MasterId, Guid>(
                    idVo => idVo.Value,
                    guid => new MasterId(guid)
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
