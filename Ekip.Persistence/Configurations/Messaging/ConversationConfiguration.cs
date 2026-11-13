using Ekip.Domain.Entities.Messaging;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Messaging;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable(nameof(Conversation));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.ConversationType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.LastMessagePreview)
            .HasMaxLength(200);

        // JSONB Mapping for ChatTheme Value Object
        builder.OwnsOne(x => x.Theme, nav =>
        {
            nav.ToJson();
        });

        // Relationships
        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithOne()
            .HasForeignKey<Conversation>(x => x.EkipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantOneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantTwoId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1. Unique EkipGroup conversation per Ekip
        builder.HasIndex(x => x.EkipId)
            .IsUnique()
            .HasFilter("ekip_id IS NOT NULL");

        // 2. O(1) Unique Direct Chat Lookup between two users
        builder.HasIndex(x => new { x.ParticipantOneId, x.ParticipantTwoId })
            .IsUnique()
            .HasFilter("participant_one_id IS NOT NULL AND participant_two_id IS NOT NULL");

        // 3. Ordering chat list by latest message
        builder.HasIndex(x => x.LastMessageAtUtc);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}