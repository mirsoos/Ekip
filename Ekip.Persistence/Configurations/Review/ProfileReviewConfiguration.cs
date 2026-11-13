using Ekip.Domain.Entities.Review;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Review;

public sealed class ProfileReviewConfiguration : IEntityTypeConfiguration<ProfileReview>
{
    public void Configure(EntityTypeBuilder<ProfileReview> builder)
    {
        builder.ToTable(nameof(ProfileReview));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.CommentText)
            .HasMaxLength(1000);

        builder.Property(x => x.ToxicityScore)
            .HasPrecision(5, 4);

        builder.Property(x => x.ModerationStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithMany()
            .HasForeignKey(x => x.EkipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        // A user can review a target user only once per Ekip
        builder.HasIndex(x => new { x.EkipId, x.AuthorUserId, x.TargetUserId }).IsUnique();
        builder.HasIndex(x => new { x.TargetUserId, x.ModerationStatus });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}