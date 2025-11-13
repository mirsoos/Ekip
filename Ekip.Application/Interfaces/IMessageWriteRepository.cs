using Ekip.Domain.Entities.Chat.Entites;


namespace Ekip.Application.Interfaces
{
    public interface IMessageWriteRepository
    {
        Task<Message> AddAsync(Message message,CancellationToken cancellationToken);
    }
}
