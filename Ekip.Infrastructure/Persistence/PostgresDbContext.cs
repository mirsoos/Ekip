using Ekip.Application.DTOs.Request;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Persistence
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
    }
}
