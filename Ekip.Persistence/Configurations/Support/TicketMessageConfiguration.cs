using Ekip.Domain.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Support;

public sealed class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
    public void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        builder.ToTable(nameof(TicketMessage));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.MessageText)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.AttachmentAssetIds)
            .HasField("_attachmentAssetIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("uuid[]");

        builder.HasIndex(x => new { x.TicketId, x.CreatedAtUtc });
    }
}