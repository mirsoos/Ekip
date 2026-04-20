using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Persistence.PostgreSql.Contexts
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options)
        {

        }

        public DbSet<ChatRoomReadModel> ChatRoomReads { get; set; }
        public DbSet<MessageReadModel> MessageReads { get; set; }
        public DbSet<ProfileReadModel> ProfileReads { get; set; }
        public DbSet<RequestReadModel> RequestReads { get; set; }
        public DbSet<UserReadModel> UserReads { get; set; }
        public DbSet<RequestAssignmentReadModel> RequestAssignmentReads { get; set; }
        public DbSet<UserEkipReadModel> userEkipReads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestReadModel>()
            .Property(r => r.RequestFilters)
            .HasColumnType("jsonb");

            modelBuilder.Entity<UserEkipReadModel>(entity =>
            {
                entity.ToTable("UserEkipReads");

                entity.HasKey(e => e.RequestRef);

                entity.HasIndex(e => e.CreatorRef)
                    .HasDatabaseName("IX_UserEkipReads_CreatorRef");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_UserEkipReads_Status");

                entity.HasIndex(e => e.RequestDateTime)
                    .HasDatabaseName("IX_UserEkipReads_RequestDateTime");

                entity.HasIndex(e => e.IsDeleted)
                    .HasDatabaseName("IX_UserEkipReads_IsDeleted");

                entity.HasIndex(e => new { e.CreatorRef, e.Status, e.IsDeleted })
                    .HasDatabaseName("IX_UserEkipReads_Creator_Status_Deleted");

                entity.Property(e => e.PendingAssignments)
                    .HasColumnType("jsonb")
                    .IsRequired(false);

                entity.Property(e => e.AcceptedMembers)
                    .HasColumnType("jsonb")
                    .IsRequired(false);

                entity.Property(e => e.EkipTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatorName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Tags)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatorAvatar)
                    .HasMaxLength(500);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.CurrentMembersCount)
                    .HasDefaultValue(0);
            });

        }

    }
}
