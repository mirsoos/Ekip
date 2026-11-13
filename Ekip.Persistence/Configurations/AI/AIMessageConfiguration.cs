using Ekip.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.AI;

public sealed class AIMessageConfiguration : IEntityTypeConfiguration<AIMessage>
{
    public void Configure(EntityTypeBuilder<AIMessage> builder)
    {
        builder.ToTable(nameof(AIMessage));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(8000);

        // PostgreSQL native uuid[] with Backing Field
        builder.Property(x => x.SuggestedEkipIds)
            .HasField("_suggestedEkipIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("uuid[]");

        builder.HasOne<AIConversation>()
            .WithMany()
            .HasForeignKey(x => x.AIConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => new { x.AIConversationId, x.CreatedAtUtc });
    }
}