using Ekip.Domain.Entities.Admin;
using Ekip.Domain.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.Configuration;

public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable(nameof(SystemSetting));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.ValueJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne<AdminAccount>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AdminAccount>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.IsPublic).HasFilter("is_public = true");
    }
}