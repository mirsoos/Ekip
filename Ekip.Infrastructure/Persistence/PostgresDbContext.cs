using Ekip.Domain.Entities.Chat.Entites;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Persistence
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options)
            {

            }

        public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
    }
}
