using Ekip.Domain.Entities.Subscription;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Subscription;

public sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable(nameof(UserSubscription));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.OwnsOne(x => x.QuotaUsage, nav =>
        {
            nav.ToJson();
        });

        builder.HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<UserSubscription>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SubscriptionPlan>()
            .WithMany()
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}