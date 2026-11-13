using Ekip.Domain.Entities.Ekip;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Ekip;

public sealed class EkipHostCompanionConfiguration : IEntityTypeConfiguration<EkipHostCompanion>
{
    public void Configure(EntityTypeBuilder<EkipHostCompanion> builder)
    {
        builder.ToTable(nameof(EkipHostCompanion));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.CompanionStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.CompanionUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.EkipId, x.CompanionUserId }).IsUnique();
    }
}