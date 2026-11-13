using Ekip.Domain.Entities.Ekip;
using Ekip.Domain.Entities.User;
using Ekip.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Ekip;

public sealed class EkipMemberConfiguration : IEntityTypeConfiguration<EkipMember>
{
    public void Configure(EntityTypeBuilder<EkipMember> builder)
    {
        builder.ToTable(nameof(EkipMember));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.MembershipStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AttendanceStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // PostGIS Geography Point for GPS Check-In
        builder.Property(x => x.CheckInLocation)
            .HasConversion<GeoLocationValueConverter>()
            .HasColumnType("geography(Point, 4326)");

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithMany()
            .HasForeignKey(x => x.EkipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.EkipApplication.EkipApplication>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.EkipId, x.UserId }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.MembershipStatus });
    }
}