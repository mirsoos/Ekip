using Ekip.Domain.Entities.Gamification;

namespace Ekip.Application.Interfaces.Repositories.Gamification;

public interface IGamificationRepository
{
    Task<bool> SummaryExistsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserGamificationSummary?> GetSummaryForUpdateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> LedgerEntryExistsAsync(Guid idempotencyKey, CancellationToken cancellationToken = default);

    void AddSummary(UserGamificationSummary summary);
    void AddLedgerEntry(ReputationLedger ledgerEntry);
}