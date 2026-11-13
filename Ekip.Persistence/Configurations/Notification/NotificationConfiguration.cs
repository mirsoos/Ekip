using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Notification;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Notification.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Notification.Notification> builder)
    {
        builder.ToTable(nameof(Domain.Entities.Notification.Notification));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.DeepLink)
            .HasMaxLength(500);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.RecipientId, x.IsRead, x.CreatedAtUtc });
    }
}