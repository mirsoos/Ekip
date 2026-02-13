using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Infrastructure.Persistence.MongoDb.Contexts;
using MongoDB.Driver;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ChatRoomWriteRepository : IChatRoomWriteRepository
    {
        private readonly MongoDbContext _mongoDb;
        public ChatRoomWriteRepository(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<ChatRoom> AddChatRoomAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
        {
            await _mongoDb.ChatRooms.InsertOneAsync(chatRoom, cancellationToken: cancellationToken);
            return chatRoom;
        }

        public async Task<ChatRoom?> GetByIdAsync(Guid chatroomRef, CancellationToken cancellationToken)
        {
           var filter = Builders<ChatRoom>.Filter.Eq(x=>x.Id , chatroomRef);
           return await _mongoDb.ChatRooms.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
