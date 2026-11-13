using Ekip.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Location;

public sealed class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable(nameof(Province));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.NativeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(x => x.Cities)
            .WithOne()
            .HasForeignKey(c => c.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Cities)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.CountryId, x.EnglishName }).IsUnique();
        builder.HasIndex(x => new { x.CountryId, x.IsActive, x.DisplayOrder });
    }
}