using Ekip.Domain.Entities.AI;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.AI;

public sealed class AIConversationConfiguration : IEntityTypeConfiguration<AIConversation>
{
    public void Configure(EntityTypeBuilder<AIConversation> builder)
    {
        builder.ToTable(nameof(AIConversation));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Title)
            .HasMaxLength(200);

        builder.Property(x => x.ContextMode)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.UpdatedAtUtc });

        // Partial Index for background worker to quickly find conversations needing memory extraction
        builder.HasIndex(x => x.IsMemoryExtracted)
            .HasFilter("is_memory_extracted = false");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}