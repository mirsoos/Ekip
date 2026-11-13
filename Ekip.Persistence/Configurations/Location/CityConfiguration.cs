using Ekip.Domain.Entities.Location;
using Ekip.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Location;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable(nameof(City));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.NativeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(100);

        // PostGIS Geography Point for City Center
        builder.Property(x => x.CenterPoint)
            .HasConversion<GeoLocationValueConverter>()
            .HasColumnType("geography(Point, 4326)")
            .IsRequired();

        builder.HasOne<Country>()
            .WithMany()
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique city name within the same province (prevents collision of same city names across different provinces)
        builder.HasIndex(x => new { x.ProvinceId, x.EnglishName }).IsUnique();
        builder.HasIndex(x => new { x.ProvinceId, x.IsActive, x.DisplayOrder });
        builder.HasIndex(x => new { x.CountryId, x.IsActive });
        builder.HasIndex(x => x.CenterPoint).HasMethod("gist");
    }
}