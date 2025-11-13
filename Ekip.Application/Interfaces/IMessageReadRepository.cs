using Ekip.Domain.Entities.Chat.Entites;


namespace Ekip.Application.Interfaces
{
    public interface IMessageReadRepository
    {
        Task<List<Message>> GetMessagesAsync(long chatRoomId,int Take = 50, CancellationToken cancellationToken = default);
    }
}
