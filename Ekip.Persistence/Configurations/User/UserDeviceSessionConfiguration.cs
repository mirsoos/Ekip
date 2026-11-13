using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.User;

public sealed class UserDeviceSessionConfiguration : IEntityTypeConfiguration<UserDeviceSession>
{
    public void Configure(EntityTypeBuilder<UserDeviceSession> builder)
    {
        builder.ToTable(nameof(UserDeviceSession));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.DeviceId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.DeviceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .HasMaxLength(150);

        builder.Property(x => x.AppVersion)
            .HasMaxLength(30);

        builder.Property(x => x.FcmPushToken)
            .HasMaxLength(512);

        builder.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64);

        builder.Property(x => x.RevokedReason)
            .HasMaxLength(100);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Hardware DeviceId deduplication per user
        builder.HasIndex(x => new { x.UserId, x.DeviceId }).IsUnique();
        builder.HasIndex(x => x.RefreshTokenHash).IsUnique();
        builder.HasIndex(x => x.FcmPushToken).HasFilter("fcm_push_token IS NOT NULL");
    }
}