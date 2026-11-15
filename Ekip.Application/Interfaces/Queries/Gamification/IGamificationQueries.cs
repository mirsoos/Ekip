using Ekip.Application.Contracts;
using Ekip.Application.DTOs.Gamification;

namespace Ekip.Application.Interfaces.Queries.Gamification;

public interface IGamificationQueries
{
    Task<CursorPagedResult<ReputationLedgerItemDto>> GetReputationHistoryAsync(
        Guid userId,
        string? cursor,
        int pageSize,
        CancellationToken cancellationToken = default);
}