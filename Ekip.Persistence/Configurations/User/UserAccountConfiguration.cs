using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.User;

public sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable(nameof(UserAccount));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512);

        builder.Property(x => x.AccountStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.NationalIdHash)
            .HasMaxLength(128);

        builder.Property(x => x.ReferralCode)
            .IsRequired()
            .HasMaxLength(20);

        // JSONB Mapping for OAuthProviders
        builder.OwnsMany(x => x.OAuthProviders, nav =>
        {
            nav.ToJson();
        });
        builder.Navigation(x => x.OAuthProviders).HasField("_oAuthProviders").UsePropertyAccessMode(PropertyAccessMode.Field);
        // Self-Referencing Referral FK
        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ReferredById)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.PhoneNumber).IsUnique().HasFilter("phone_number IS NOT NULL");
        builder.HasIndex(x => x.Email).IsUnique().HasFilter("email IS NOT NULL");
        builder.HasIndex(x => x.NationalIdHash).IsUnique().HasFilter("national_id_hash IS NOT NULL");
        builder.HasIndex(x => x.ReferralCode).IsUnique();

        // Soft Delete Filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}