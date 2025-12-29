using Ekip.Domain.Entities.Chat.Entites;
using Ekip.Domain.Entities.ReadModels;


namespace Ekip.Application.Interfaces
{
    public interface IMessageReadRepository
    {
        Task<List<MessageReadModel>> GetMessagesAsync(long chatRoomRef,int Take = 50, CancellationToken cancellationToken = default);
        Task<MessageReadModel> AddMessageAsync(MessageReadModel messageReadModel, CancellationToken cancellationToken);
    }
}
