
using Ekip.Domain.Entities.UserBehavior.Entities;

namespace Ekip.Application.Interfaces
{
    public interface IScoreLedgerWriteRepository
    {
        Task AddScoreAsync(ScoreLedger scoreLedger, CancellationToken cancellationToken);
    }
}