using Ekip.Domain.Entities.User;
using Ekip.Domain.Enums.EkipApplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.EkipApplication;

public sealed class EkipApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.EkipApplication.EkipApplication>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.EkipApplication.EkipApplication> builder)
    {
        builder.ToTable(nameof(Domain.Entities.EkipApplication.EkipApplication));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Ignore(x => x.PartySize);

        builder.Property(x => x.JoinType)
            .IsRequired();

        builder.Property(x => x.IntroNote)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ApplicationStatus)
            .IsRequired();

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithMany()
            .HasForeignKey(x => x.EkipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Child collection mapping for Companions
        builder.HasMany(x => x.Companions)
            .WithOne()
            .HasForeignKey(c => c.EkipApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Companions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        // FIFO Waitlist & Host pending list index
        builder.HasIndex(x => new { x.EkipId, x.ApplicationStatus, x.CreatedAtUtc });

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.ApplicantId, x.EkipId })
            .IsUnique()
            .HasFilter(
                $"is_deleted = false AND application_status IN ({(int)EkipApplicationStatus.WaitingCompanionApproval}, {(int)EkipApplicationStatus.PendingHostApproval}, {(int)EkipApplicationStatus.Waitlisted}, {(int)EkipApplicationStatus.Approved})");

        builder.HasIndex(x => new { x.ApplicantId, x.CreatedAtUtc });

    }
}