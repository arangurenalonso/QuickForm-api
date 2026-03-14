using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;

public class AuthActionTokenConfiguration : EntityMapBase<AuthActionTokenDomain, AuthActionTokenId>
{
    protected override void Configure(EntityTypeBuilder<AuthActionTokenDomain> builder)
    {
        builder.ToTable("AuthActionTokens");

        builder.HasKey(uat => uat.Id);

        builder.Property(uat => uat.Id)
            .HasConversion(
                new ValueConverter<AuthActionTokenId, Guid>(
                    idVO => idVO.Value,
                    guid => new AuthActionTokenId(guid)))
            .IsRequired();

        builder.Property(uat => uat.IdUser)
            .HasConversion(
                new ValueConverter<UserId, Guid>(
                    userId => userId.Value,
                    guid => new UserId(guid)))
            .IsRequired();

        builder.Property(uat => uat.IdUserAction)
            .HasConversion(
                new ValueConverter<MasterId, Guid>(
                    userActionId => userActionId.Value,
                    guid => new MasterId(guid)))
            .IsRequired();

        builder.Property(p => p.TokenHash)
            .HasColumnName("TokenHash")
            .HasMaxLength(200)
            .IsRequired();

        builder.Ignore(p => p.PlainTextToken);

        builder.Property(p => p.ExpiresAt)
            .HasColumnName("ExpiresAt")
            .IsRequired()
            .HasConversion(
                new ValueConverter<ExpirationDate, DateTime>(
                    dateEnd => dateEnd.Value,
                    date => ExpirationDate.Restore(date)));

        builder.Property(uat => uat.Used)
            .HasColumnName("Used")
            .IsRequired();

        builder.HasOne(uat => uat.User)
            .WithMany(u => u.AuthActionTokens)
            .HasForeignKey(uat => uat.IdUser)
            .IsRequired();

        builder.HasOne(uat => uat.Action)
            .WithMany(ua => ua.UserActionTokens)
            .HasForeignKey(uat => uat.IdUserAction)
            .IsRequired();

        builder.HasIndex(x => new { x.IdUser, x.IdUserAction, x.Used, x.ExpiresAt });
        builder.HasIndex(x => new { x.IdUserAction, x.TokenHash });
    }
}
