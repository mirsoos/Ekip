using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;


namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class PostgreMessageRepository : IMessageReadRepository
    {
        public Task<MessageReadModel> AddMessageAsync(MessageReadModel messageReadModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Message>> GetMessagesAsync(long chatRoomRef, int take = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
