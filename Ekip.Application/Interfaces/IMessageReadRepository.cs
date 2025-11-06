using Ekip.Domain.Entities;


namespace Ekip.Application.Interfaces
{
    public interface IMessageReadRepository
    {
        Task<List<Message>> GetMessagesAsync(long chatRoomId,int Take = 50, CancellationToken cancellationToken = default);
    }
}
