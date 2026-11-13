using Ekip.Domain.Entities.AI;
using Ekip.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.AI;

public sealed class UserIdentityVectorConfiguration : IEntityTypeConfiguration<UserIdentityVector>
{
    public void Configure(EntityTypeBuilder<UserIdentityVector> builder)
    {
        builder.ToTable(nameof(UserIdentityVector));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        // PostgreSQL pgvector (1536 dimensions)
        builder.Property(x => x.PersonalityVector)
            .HasColumnType("vector(1536)")
            .IsRequired();

        builder.HasIndex(x => x.PersonalityVector)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        builder.Property(x => x.AiPersonaSummary)
            .HasMaxLength(4000);

        builder.Property(x => x.UserGender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<UserAccount>()
            .WithOne()
            .HasForeignKey<UserIdentityVector>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Pre-filtering index for matchmaking
        builder.HasIndex(x => new { x.CityId, x.UserGender, x.BirthYear });
    }
}