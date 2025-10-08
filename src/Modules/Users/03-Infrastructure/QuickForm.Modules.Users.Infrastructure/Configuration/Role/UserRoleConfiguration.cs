using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class UserRoleConfiguration : EntityMapBase<UserRoleDomain, UserRoleId>
{
    protected override void Configure(EntityTypeBuilder<UserRoleDomain> builder)
    {
        builder.ToTable("UserRole");

        builder.HasKey(uat => uat.Id);

        builder.Property(uat => uat.Id)
            .HasConversion(
                new ValueConverter<UserRoleId, Guid>(
                    idVO => idVO.Value,
                    guid => new UserRoleId(guid)
                ))
            .IsRequired();

        builder.Property(uat => uat.IdUser)
            .HasConversion(
                new ValueConverter<UserId, Guid>(
                    idVO => idVO.Value,
                    guid => new UserId(guid)
                ))
            .IsRequired();


        builder.Property(uat => uat.IdRole)
            .HasConversion(
                new ValueConverter<MasterId, Guid>(
                    idVO => idVO.Value,
                    guid => new MasterId(guid)
                ))
            .IsRequired();


        builder.HasOne(ua => ua.User)
            .WithMany(uat => uat.UserRole)
            .HasForeignKey(uat => uat.IdUser)
            .IsRequired();
        builder.HasOne(ua => ua.Role)
            .WithMany(uat => uat.UserRole)
            .HasForeignKey(uat => uat.IdRole)
            .IsRequired();

    }

}
