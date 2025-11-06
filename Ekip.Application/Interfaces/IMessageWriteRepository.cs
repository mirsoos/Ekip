using Ekip.Domain.Entities;


namespace Ekip.Application.Interfaces
{
    public interface IMessageWriteRepository
    {
        Task<Message> AddAsync(Message message,CancellationToken cancellationToken);
    }
}
