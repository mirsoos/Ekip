using Ekip.Domain.Entities.Billing;
using Ekip.Domain.Entities.Discount;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Billing;

public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable(nameof(PaymentTransaction));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Purpose)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.BaseAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.WalletAmountUsed)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.GatewayPayableAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Gateway)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AuthorityOrToken)
            .HasMaxLength(256);

        builder.Property(x => x.ReferenceId)
            .HasMaxLength(256);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PromoCode>()
            .WithMany()
            .HasForeignKey(x => x.PromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => x.AuthorityOrToken).HasFilter("authority_or_token IS NOT NULL");
        builder.HasIndex(x => x.ReferenceId).IsUnique().HasFilter("reference_id IS NOT NULL");
        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
    }
}