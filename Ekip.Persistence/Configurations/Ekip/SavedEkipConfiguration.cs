using Ekip.Domain.Entities.Ekip;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Ekip;

public sealed class SavedEkipConfiguration : IEntityTypeConfiguration<SavedEkip>
{
    public void Configure(EntityTypeBuilder<SavedEkip> builder)
    {
        builder.ToTable(nameof(SavedEkip));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithMany()
            .HasForeignKey(x => x.EkipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.EkipId }).IsUnique();
    }
}