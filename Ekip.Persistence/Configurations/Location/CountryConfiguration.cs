using Ekip.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Location;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable(nameof(Country));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.NativeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsoCode)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(x => x.PhonePrefix)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.DefaultTimeZoneId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(x => x.Provinces)
            .WithOne()
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Provinces)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.IsoCode).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.DisplayOrder });
    }
}