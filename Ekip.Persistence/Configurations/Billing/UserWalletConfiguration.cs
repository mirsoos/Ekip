using Ekip.Domain.Entities.Billing;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Billing;

public sealed class UserWalletConfiguration : IEntityTypeConfiguration<UserWallet>
{
    public void Configure(EntityTypeBuilder<UserWallet> builder)
    {
        builder.ToTable(nameof(UserWallet));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Ignore(x => x.AvailableBalance);

        builder.Property(x => x.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.LockedBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<UserWallet>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}