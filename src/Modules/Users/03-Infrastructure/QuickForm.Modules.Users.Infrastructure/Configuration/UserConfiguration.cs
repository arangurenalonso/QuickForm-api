using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;
public class UserConfiguration : EntityMapBase<UserDomain, UserId>
{
    protected override void Configure(EntityTypeBuilder<UserDomain> builder)
    {
        builder.ToTable("Users");

        builder.Property(p => p.Id)
            .HasConversion(
                new ValueConverter<UserId, Guid>(
                    userId => userId.Value,
                    guid => new UserId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.OwnsOne(p => p.PasswordHash, passwordBuilder =>
        {
            passwordBuilder.Property(p => p.Value)
                .HasColumnName("PasswordHash")
                .IsRequired();
        });

        builder.OwnsOne(p => p.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("Name");
        });

        builder.OwnsOne(p => p.LastName, lastNameBuilder =>
        {
            lastNameBuilder.Property(l => l.Value)
                .HasColumnName("LastName");
        });
        builder.HasMany(ua => ua.AuthActionTokens)
            .WithOne(uat => uat.User)
            .HasForeignKey(uat => uat.IdUser)
            .IsRequired();
    }
}
