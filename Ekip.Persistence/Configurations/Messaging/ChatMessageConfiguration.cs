using Ekip.Domain.Entities.Messaging;
using Ekip.Domain.Entities.User;
using Ekip.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Messaging;

public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable(nameof(ChatMessage));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ContentText)
            .HasMaxLength(4000);

        // PostGIS Geography Point for Location messages
        builder.Property(x => x.Location)
            .HasConversion<GeoLocationValueConverter>()
            .HasColumnType("geography(Point, 4326)");

        // JSONB Mapping for ChatAttachment Value Object
        builder.OwnsOne(x => x.Attachment, nav =>
        {
            nav.ToJson();
        });

        builder.HasOne<Conversation>()
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing FK for Replies
        builder.HasOne<ChatMessage>()
            .WithMany()
            .HasForeignKey(x => x.ReplyToMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        // Client Idempotency Key
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        // Fast pagination of messages inside a conversation (UUIDv7 / CreatedAtUtc)
        builder.HasIndex(x => new { x.ConversationId, x.CreatedAtUtc });

        // Fast lookup for Pinned messages in a conversation
        builder.HasIndex(x => new { x.ConversationId, x.IsPinned })
            .HasFilter("is_pinned = true");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}