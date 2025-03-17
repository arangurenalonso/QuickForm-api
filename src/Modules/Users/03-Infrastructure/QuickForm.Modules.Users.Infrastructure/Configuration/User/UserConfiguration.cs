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

        builder.Property(p => p.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasConversion(
                    passwordVO => passwordVO.Value,
                    passwordString => PasswordVO.Create(passwordString,null).Value
                    );

        builder.Property(p => p.Name)
                .HasColumnName("Name")
                .HasConversion(
                    nameVO => nameVO.Value,
                    nameString => NameVO.Create(nameString).Value
                    );

        builder.Property(p => p.LastName)
                .HasColumnName("LastName")
                .HasConversion(
                    lastNameVO => lastNameVO==null?null: lastNameVO.Value,
                    lastNameString => LastNameVO.Create(lastNameString).Value
                    );

        builder.HasMany(ua => ua.AuthActionTokens)
            .WithOne(uat => uat.User)
            .HasForeignKey(uat => uat.IdUser)
            .IsRequired();
    }
}
