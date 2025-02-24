using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace QuickForm.Common.Infrastructure;
public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages");

        builder.HasKey(o => o.Id);

        builder.Property(inboxMessage => inboxMessage.Content).HasColumnType("nvarchar(max)");
    }
}
