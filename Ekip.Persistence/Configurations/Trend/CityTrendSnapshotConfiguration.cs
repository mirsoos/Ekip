using Ekip.Domain.Entities.Location;
using Ekip.Domain.Entities.Trend;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Trend;

public sealed class CityTrendSnapshotConfiguration : IEntityTypeConfiguration<CityTrendSnapshot>
{
    public void Configure(EntityTypeBuilder<CityTrendSnapshot> builder)
    {
        builder.ToTable(nameof(CityTrendSnapshot));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Keyword)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.TrendScore)
            .HasPrecision(10, 4)
            .IsRequired();

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CityId, x.CreatedAtUtc, x.Rank });
    }
}