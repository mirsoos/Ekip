using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Infrastructure.Persistence;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomWriteRepository : IChatRoomWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public async Task<ChatRoom> AddChatRoomAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
        {
            await _mongoDb.ChatRooms.InsertOneAsync(chatRoom, cancellationToken: cancellationToken);
            return chatRoom;
        }
    }
}
