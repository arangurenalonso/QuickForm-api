using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class PermissionsActionsConfiguration : EntityMapBase<PermissionsActionsDomain, PermissionsActionsId>
{
    protected override void Configure(EntityTypeBuilder<PermissionsActionsDomain> builder)
    {
        builder.ToTable("PermissionsActions");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<PermissionsActionsId, Guid>(
                    idVO => idVO.Value,
                    guid => new PermissionsActionsId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => PermissionsActionsDescription.Create(descriptionString).Value
                    );

    }
}
