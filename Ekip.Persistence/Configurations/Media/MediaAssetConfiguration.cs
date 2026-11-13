using Ekip.Domain.Entities.Media;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Media;

public sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable(nameof(MediaAsset));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.UsageContext)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.NsfwProbability)
            .HasPrecision(5, 4);

        builder.Property(x => x.ModerationStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(1000);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.StorageKey).IsUnique();
        builder.HasIndex(x => new { x.UploaderId, x.ModerationStatus });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}