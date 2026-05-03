
namespace Ekip.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task Publish<T>(T message,CancellationToken cancellationToken) where T : class;
    }
}
