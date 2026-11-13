using Ekip.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Admin;

public sealed class AdminSessionConfiguration : IEntityTypeConfiguration<AdminSession>
{
    public void Configure(EntityTypeBuilder<AdminSession> builder)
    {
        builder.ToTable(nameof(AdminSession));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.HasOne<AdminAccount>()
            .WithMany()
            .HasForeignKey(x => x.AdminId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RefreshTokenHash).IsUnique();
        builder.HasIndex(x => new { x.AdminId, x.IsRevoked });
    }
}