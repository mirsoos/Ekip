using Ekip.Domain.Entities.Location;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.User;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(nameof(UserProfile));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UserGender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasMaxLength(1000);

        builder.Property(x => x.JobTitle)
            .HasMaxLength(150);

        builder.Property(x => x.MbtiTypes)
            .HasConversion<string>()
            .HasMaxLength(10);

        // PostgreSQL native varchar[] with GIN Index for Interests
        builder.Property(x => x.Interests)
            .HasField("_interests")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("varchar(100)[]");

        builder.HasIndex(x => x.Interests)
            .HasMethod("gin");

        // JSONB Value Objects
        builder.OwnsOne(x => x.MoodWidget, nav =>
        {
            nav.ToJson();
        });

        builder.OwnsMany(x => x.SocialLinks, nav =>
        {
            nav.ToJson();
        });
        builder.Navigation(x => x.SocialLinks).HasField("_socialLinks").UsePropertyAccessMode(PropertyAccessMode.Field);
        // 1-to-1 Relationship with UserAccount
        builder.HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with City
        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.CityId);
    }
}