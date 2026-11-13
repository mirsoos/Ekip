using Ekip.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Admin;

public sealed class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> builder)
    {
        builder.ToTable(nameof(AdminAuditLog));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ActionType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TargetEntity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OldValueJson)
            .HasColumnType("jsonb");

        builder.Property(x => x.NewValueJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.AdminIpAddress)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne<AdminAccount>()
            .WithMany()
            .HasForeignKey(x => x.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AdminId, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.TargetEntity, x.TargetId });
    }
}