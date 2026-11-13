using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.User;

public sealed class UserRelationConfiguration : IEntityTypeConfiguration<UserRelation>
{
    public void Configure(EntityTypeBuilder<UserRelation> builder)
    {
        builder.ToTable(nameof(UserRelation));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate relation records between the same pair
        builder.HasIndex(x => new { x.RequesterId, x.ReceiverId }).IsUnique();
        builder.HasIndex(x => new { x.ReceiverId, x.Status });
    }
}