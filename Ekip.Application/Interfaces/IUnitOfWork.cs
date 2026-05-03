
namespace Ekip.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
    }
}