using Ekip.Domain.Entities.EkipApplication;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.EkipApplication;

public sealed class ApplicationCompanionConfiguration : IEntityTypeConfiguration<ApplicationCompanion>
{
    public void Configure(EntityTypeBuilder<ApplicationCompanion> builder)
    {
        builder.ToTable(nameof(ApplicationCompanion));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.CompanionUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.EkipApplicationId, x.CompanionUserId }).IsUnique();
        builder.HasIndex(x => new { x.CompanionUserId, x.Status });
    }
}