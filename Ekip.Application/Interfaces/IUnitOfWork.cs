
namespace Ekip.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    }
}