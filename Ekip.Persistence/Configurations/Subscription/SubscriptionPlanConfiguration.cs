using Ekip.Domain.Entities.Subscription;
using Ekip.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Subscription;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable(nameof(SubscriptionPlan));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        // Complex Property (Flat columns on table) for Price
        builder.ComplexProperty(x => x.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(p => p.Currency)
                .HasMaxLength(10)
                .IsRequired();
        });

        // JSONB Mapping for Entitlements
        builder.Property(x => x.Entitlements)
            .HasJsonbConversion()
            .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
    }
}