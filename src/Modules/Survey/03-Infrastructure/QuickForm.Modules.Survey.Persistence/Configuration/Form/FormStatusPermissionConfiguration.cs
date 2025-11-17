using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Persistence;
public class FormStatusPermissionConfiguration : EntityMapBase<FormStatusPermissionDomain, FormStatusPermissionId>
{
    protected override void Configure(EntityTypeBuilder<FormStatusPermissionDomain> builder)
    {
        builder.ToTable("FormStatusPermission");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<FormStatusPermissionId, Guid>(
                    idVO => idVO.Value,
                    guid => new FormStatusPermissionId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.HasOne(permission => permission.FormAction)
            .WithMany(action => action.Permissions)
            .HasForeignKey(section => section.IdFormAction)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasOne(permission => permission.FormStatus)
            .WithMany(status => status.Permissions)
            .HasForeignKey(section => section.IdFormStatus)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);


    }
}
