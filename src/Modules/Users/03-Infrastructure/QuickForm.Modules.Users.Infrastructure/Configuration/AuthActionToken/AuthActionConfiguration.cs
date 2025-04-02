using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
using QuickForm.Common.Infrastructure.Persistence;

namespace QuickForm.Modules.Users.Persistence;
public class AuthActionConfiguration : EntityMapBase<AuthActionDomain, AuthActionId> 
{
    protected override void Configure(EntityTypeBuilder<AuthActionDomain> builder)
    {
        builder.ToTable("AuthActions");

        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.Id)
            .HasConversion(
                new ValueConverter<AuthActionId, Guid>(
                    idVO => idVO.Value,
                    guid => new AuthActionId(guid)
                ))
            .IsRequired();


        builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    descriptionVO => descriptionVO.Value,
                    descriptionString => ActionDescriptionVO.Create(descriptionString).Value
                    );

        builder.HasMany(ua => ua.UserActionTokens)
            .WithOne(uat => uat.Action)
            .HasForeignKey(uat => uat.IdUserAction)
            .IsRequired();
    }
}
