using Ekip.Domain.Entities.Billing;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Billing;

public sealed class WalletLedgerConfiguration : IEntityTypeConfiguration<WalletLedger>
{
    public void Configure(EntityTypeBuilder<WalletLedger> builder)
    {
        builder.ToTable(nameof(WalletLedger));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.AmountDelta)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.BalanceAfter)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
    }
}