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
                    idVO => idVO.Value,
                    guid => new UserId(guid)
                ))
            .IsRequired();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.Email)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired()
                .HasConversion(
                    emailVO => emailVO.Value, 
                    emailString => EmailVO.Create(emailString).Value
                    );

        builder.HasIndex(p => p.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

        builder.Property(p => p.IsEmailVerify)
                .HasColumnName("IsEmailVerify")
                .IsRequired();

        builder.Property(p => p.IsPasswordChanged)
                .HasColumnName("IsPasswordChanged")
                .IsRequired();

        builder.Property(p => p.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasConversion(
                    passwordVO => passwordVO.Value,
                    passwordString => PasswordVO.Restore(passwordString)
                    );

        builder.HasMany(ua => ua.AuthActionTokens)
            .WithOne(uat => uat.User)
            .HasForeignKey(uat => uat.IdUser)
            .IsRequired();

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.IdUser)
            .IsRequired();
    }
}
