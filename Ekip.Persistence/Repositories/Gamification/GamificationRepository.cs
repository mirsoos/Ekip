using Ekip.Application.Interfaces.Repositories.Gamification;
using Ekip.Domain.Entities.Gamification;
using Ekip.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Persistence.Repositories.Gamification;

internal sealed class GamificationRepository(EkipDbContext dbContext) : IGamificationRepository
{
    public Task<bool> SummaryExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserGamificationSummary>()
            .AnyAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<UserGamificationSummary?> GetSummaryForUpdateAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM user_gamification_summary WHERE user_id = {userId} FOR UPDATE",
            cancellationToken);

        return await dbContext.Set<UserGamificationSummary>()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public Task<bool> LedgerEntryExistsAsync(
        Guid idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Set<ReputationLedger>()
            .AnyAsync(l => l.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public void AddSummary(UserGamificationSummary summary)
    {
        dbContext.Set<UserGamificationSummary>().Add(summary);
    }

    public void AddLedgerEntry(ReputationLedger ledgerEntry)
    {
        dbContext.Set<ReputationLedger>().Add(ledgerEntry);
    }
}