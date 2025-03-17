using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class AuthActionConfiguration : IEntityTypeConfiguration<AuthActionDomain>
{
    public void Configure(EntityTypeBuilder<AuthActionDomain> builder)
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

        builder.OwnsOne(ua => ua.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(d => d.Value)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.HasMany(ua => ua.UserActionTokens)
            .WithOne(uat => uat.Action)
            .HasForeignKey(uat => uat.IdUserAction)
            .IsRequired();
    }
}
