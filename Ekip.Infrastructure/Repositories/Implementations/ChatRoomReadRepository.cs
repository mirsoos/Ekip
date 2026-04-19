using Ekip.Application.DTOs.ChatRoom;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomReadRepository : IChatRoomReadRepository

    {
        private readonly PostgresDbContext _postgreDb;
        public ChatRoomReadRepository(PostgresDbContext postgresDb)
        {
            _postgreDb = postgresDb;
        }

        public async Task<ChatRoomReadModel> AddChatRoomAsync(ChatRoomReadModel chatRoomReadModel, CancellationToken cancellationToken)
        {


            try
            {
                await _postgreDb.ChatRoomReads.AddAsync(chatRoomReadModel, cancellationToken: cancellationToken);
                await _postgreDb.SaveChangesAsync(cancellationToken);
                return chatRoomReadModel;
            }
            catch(Exception ex)
            {
                var a = ex.InnerException;
                return chatRoomReadModel;
            }
        }

        public async Task<ChatRoomReadModel?> GetByIdAsync(Guid chatRoomRef, CancellationToken cancellationToken)
        {
           return await _postgreDb.ChatRoomReads.AsNoTracking().FirstOrDefaultAsync(c=>c.Id == chatRoomRef , cancellationToken);
        }

        public async Task<ChatRoomListDto?> GetListByIdAsync(Guid chatRoomRef, CancellationToken cancellationToken)
        {
            var room = await _postgreDb.ChatRoomReads.AsNoTracking().Where(c => c.Id == chatRoomRef)
                .Select(s => new ChatRoomListDto
                {
                    ChatRoomRef = s.Id,
                    AvatarUrl = s.AvatarUrl,
                    Participants = s.Participants.ToList(),
                    Name = s.Name,
                    LastMessageDate = s.LastMessageDate,
                    LastMessagePreview = s.LastMessagePreview,

                }).FirstOrDefaultAsync(cancellationToken);

            return room;
        }

        public async Task UpdateLastMessageAsync(Guid chatRoomRef, string LastMessagePreview, DateTime LastMessageDate, CancellationToken cancellationToken)
        {

            await _postgreDb.ChatRoomReads
                .Where(c => c.Id == chatRoomRef)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.LastMessagePreview, LastMessagePreview)
                    .SetProperty(m => m.LastMessageDate, LastMessageDate),
                    cancellationToken
                );
        }
        public Task<bool> IsUserParticipant(Guid chatRoomRef, Guid userRef, CancellationToken cancellationToken)
        {
            return _postgreDb.ChatRoomReads
                .AsNoTracking()
                .AnyAsync(c => c.Id == chatRoomRef && c.Participants.Contains(userRef),cancellationToken);
        }

        public async Task<List<Guid>> GetUserRoomIds(Guid userRef, CancellationToken cancellationToken)
        {
            return await _postgreDb.ChatRoomReads.AsNoTracking().Where(x => x.Participants.Contains(userRef)).Select(s=>s.Id).ToListAsync(cancellationToken);
        }
    }
}
