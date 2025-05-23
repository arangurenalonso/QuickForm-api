﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
using QuickForm.Common.Infrastructure.Persistence;

namespace QuickForm.Modules.Users.Persistence;
public class RolePermissionsConfiguration : EntityMapBase<RolePermissionsDomain, RolePermissionsId>
{
    protected override void Configure(EntityTypeBuilder<RolePermissionsDomain> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(uat => uat.Id);

        builder.Property(uat => uat.Id)
            .HasConversion(
                new ValueConverter<RolePermissionsId, Guid>(
                    idVO => idVO.Value,
                    guid => new RolePermissionsId(guid)
                ))
            .IsRequired();

        builder.Property(uat => uat.IdPermission)
            .HasConversion(
                new ValueConverter<PermissionsId, Guid>(
                    idVO => idVO.Value,
                    guid => new PermissionsId(guid)
                ))
            .IsRequired();


        builder.Property(uat => uat.IdRole)
            .HasConversion(
                new ValueConverter<RoleId, Guid>(
                    idVO => idVO.Value,
                    guid => new RoleId(guid)
                ))
            .IsRequired();


        builder.HasOne(ua => ua.Permission)
            .WithMany(uat => uat.RolePermissions)
            .HasForeignKey(uat => uat.IdPermission)
            .IsRequired();
        builder.HasOne(ua => ua.Role)
            .WithMany(uat => uat.RolePermissions)
            .HasForeignKey(uat => uat.IdRole)
            .IsRequired();

    }

}
