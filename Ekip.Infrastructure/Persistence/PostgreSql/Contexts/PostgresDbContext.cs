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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestReadModel>()
            .Property(r => r.RequestFilters)
            .HasColumnType("jsonb");
        }

    }
}
