using Ekip.Domain.Entities.Location;
using Ekip.Domain.Entities.User;
using Ekip.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Ekip;

public sealed class EkipConfiguration : IEntityTypeConfiguration<Domain.Entities.Ekip.Ekip>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Ekip.Ekip> builder)
    {
        builder.ToTable(nameof(Domain.Entities.Ekip.Ekip));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.SubCategory)
            .IsRequired()
            .HasMaxLength(100);

        // PostGIS Geography Point (SRID 4326)
        builder.Property(x => x.ExactLocationPoint)
            .HasConversion<GeoLocationValueConverter>()
            .HasColumnType("geography(Point, 4326)")
            .IsRequired();

        builder.Property(x => x.PublicMapPoint)
            .HasConversion<GeoLocationValueConverter>()
            .HasColumnType("geography(Point, 4326)")
            .IsRequired();

        builder.Property(x => x.LocationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AddressDetail)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.HostComposition)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.EstimatedCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ModerationStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.LifecycleStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(x => x.QuorumStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // PostgreSQL native arrays with Backing Fields
        builder.Property(x => x.Tags)
            .HasField("_tags")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("varchar(100)[]");

        builder.Property(x => x.MediaAssetIds)
            .HasField("_mediaAssetIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("uuid[]");

        // JoinCriteria Value Object mapped as JSONB (or ComplexProperty)
        builder.OwnsOne(x => x.JoinCriteria, nav =>
        {
            nav.ToJson();
        });

        // Relationships
        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Ekip.RecurringEkip>()
            .WithMany()
            .HasForeignKey(x => x.RecurringEkipId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        // Spatial GIST Index on PublicMapPoint for bounding-box & radius queries
        builder.HasIndex(x => x.PublicMapPoint).HasMethod("gist");

        // GIN Index on Tags array
        builder.HasIndex(x => x.Tags).HasMethod("gin");

        // Composite Index for Main Feed Query
        builder.HasIndex(x => new { x.CityId, x.LifecycleStatus, x.ModerationStatus, x.EventDateTimeUtc });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}