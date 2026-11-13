using Ekip.Domain.Entities.AI;
using Ekip.Domain.Entities.Location;
using Ekip.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekip.Persistence.Configurations.AI;

public sealed class EkipSemanticVectorConfiguration : IEntityTypeConfiguration<EkipSemanticVector>
{
    public void Configure(EntityTypeBuilder<EkipSemanticVector> builder)
    {
        builder.ToTable(nameof(EkipSemanticVector));

        builder.HasKey(x => x.EkipId);
        builder.Property(x => x.EkipId).ValueGeneratedNever();

        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(x => x.ContentVector)
            .HasPgVectorConversion(1536)
            .IsRequired();

        builder.HasOne<Domain.Entities.Ekip.Ekip>()
            .WithOne()
            .HasForeignKey<EkipSemanticVector>(x => x.EkipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        // HNSW Vector Index for semantic similarity search
        builder.HasIndex(x => x.ContentVector)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasFilter("is_available = true");

        // Pre-filtering index for active Ekips in a city
        builder.HasIndex(x => new { x.CityId, x.IsAvailable })
            .HasFilter("is_available = true");
    }
}