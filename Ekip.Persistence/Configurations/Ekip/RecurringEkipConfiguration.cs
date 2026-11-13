using Ekip.Domain.Entities.Ekip;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Ekip;

public sealed class RecurringEkipConfiguration : IEntityTypeConfiguration<RecurringEkip>
{
    public void Configure(EntityTypeBuilder<RecurringEkip> builder)
    {
        builder.ToTable(nameof(RecurringEkip));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Frequency)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithMany()
            .HasForeignKey(x => x.SourceEkipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CreatorId, x.IsActive });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}