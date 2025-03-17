using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class RoleConfiguration : EntityMapBase<RoleDomain, RoleId>
{
    protected override void Configure(EntityTypeBuilder<RoleDomain> builder)
    {
        builder.ToTable("Role");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<RoleId, Guid>(
                    idVO => idVO.Value,
                    guid => new RoleId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => RoleDescriptionVO.Create(descriptionString).Value
                    );


    }
}
