using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using Microsoft.EntityFrameworkCore;


namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class MessageReadRepository : IMessageReadRepository
    {
        private readonly PostgresDbContext _postgreDb;
        public MessageReadRepository(PostgresDbContext postgresDb)
        {
            _postgreDb = postgresDb;
        }
        public async Task<MessageReadModel> AddMessageAsync(MessageReadModel messageReadModel, CancellationToken cancellationToken)
        {
            await _postgreDb.MessageReads.AddAsync(messageReadModel, cancellationToken);
            await _postgreDb.SaveChangesAsync(cancellationToken);
            return messageReadModel;
        }
        public async Task<List<MessageReadModel>> GetMessagesAsync(Guid chatRoomRef, int take = 50, CancellationToken cancellationToken = default)
        {
           return await _postgreDb.MessageReads
                .AsNoTracking()
                .Where(m => m.ChatRoomRef == chatRoomRef)
                .OrderByDescending(m => m.SentAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
