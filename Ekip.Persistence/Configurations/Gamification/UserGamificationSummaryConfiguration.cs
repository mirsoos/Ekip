using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Gamification;

public sealed class UserGamificationSummaryConfiguration : IEntityTypeConfiguration<UserGamificationSummary>
{
    public void Configure(EntityTypeBuilder<UserGamificationSummary> builder)
    {
        builder.ToTable(nameof(UserGamificationSummary));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.PublicScore)
            .HasPrecision(4, 2);

        builder.OwnsMany(x => x.AggregatedCounters, nav =>
        {
            nav.ToJson();
        });

        builder.HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<UserGamificationSummary>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}