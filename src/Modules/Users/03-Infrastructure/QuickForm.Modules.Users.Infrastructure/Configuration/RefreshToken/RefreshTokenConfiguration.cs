using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Common.Infrastructure.Persistence;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Persistence;

public class RefreshTokenConfiguration : EntityMapBase<RefreshTokenDomain, RefreshTokenId>
{
    protected override void Configure(EntityTypeBuilder<RefreshTokenDomain> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                new ValueConverter<RefreshTokenId, Guid>(
                    id => id.Value,
                    guid => new RefreshTokenId(guid)))
            .IsRequired();

        builder.Property(x => x.IdUser)
            .HasConversion(
                new ValueConverter<UserId, Guid>(
                    id => id.Value,
                    guid => new UserId(guid)))
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("TokenHash")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.ReplacedByTokenId);

        builder.Property(x => x.FamilyId)
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(100);

        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(100);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(512);

        builder.Ignore(x => x.PlainTextToken);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.IdUser)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => x.IdUser);
        builder.HasIndex(x => x.FamilyId);
    }
}
