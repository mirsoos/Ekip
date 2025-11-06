using Ekip.Domain.Entities;


namespace Ekip.Application.Interfaces
{
    public interface IMessageReadRepository
    {
        Task<List<Message>> GetMessagesAsync(long chatRoomId, CancellationToken cancellationToken = default);
        Task<Chatroom> GetById()
    }
}
