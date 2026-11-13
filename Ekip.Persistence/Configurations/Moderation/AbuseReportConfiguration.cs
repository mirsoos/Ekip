using Ekip.Domain.Entities.Admin;
using Ekip.Domain.Entities.Moderation;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Moderation;

public sealed class AbuseReportConfiguration : IEntityTypeConfiguration<AbuseReport>
{
    public void Configure(EntityTypeBuilder<AbuseReport> builder)
    {
        builder.ToTable(nameof(AbuseReport));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.TargetType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.AdminNote)
            .HasMaxLength(2000);

        builder.Property(x => x.EvidenceMediaAssetIds)
            .HasField("_evidenceMediaAssetIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("uuid[]");

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AdminAccount>()
            .WithMany()
            .HasForeignKey(x => x.ResolvedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => new { x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.TargetType, x.TargetId });
    }
}