using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuickForm.Modules.Users.Domain;
using QuickForm.Common.Infrastructure.Persistence;

namespace QuickForm.Modules.Users.Persistence;
public class AuthActionConfiguration : MasterEntityMapBase<AuthActionDomain> 
{
    protected override void ConfigureMaster(EntityTypeBuilder<AuthActionDomain> builder)
    {
        builder.ToTable("AuthActions");


        builder.HasMany(ua => ua.UserActionTokens)
            .WithOne(uat => uat.Action)
            .HasForeignKey(uat => uat.IdUserAction)
            .IsRequired();
    }
}
