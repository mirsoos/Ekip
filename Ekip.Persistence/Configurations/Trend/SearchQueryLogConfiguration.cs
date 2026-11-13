using Ekip.Domain.Entities.Location;
using Ekip.Domain.Entities.Trend;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Trend;

public sealed class SearchQueryLogConfiguration : IEntityTypeConfiguration<SearchQueryLog>
{
    public void Configure(EntityTypeBuilder<SearchQueryLog> builder)
    {
        builder.ToTable(nameof(SearchQueryLog));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.NormalizedQuery)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CityId, x.CreatedAtUtc, x.NormalizedQuery });
    }
}