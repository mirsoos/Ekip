using Dapper;
using Ekip.Application.Contracts;
using Ekip.Application.DTOs.Gamification;
using Ekip.Application.Helpers;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Queries.Gamification;
using Ekip.Domain.Enums.Gamification;

namespace Ekip.Persistence.Queries.Gamification;

internal sealed class GamificationQueries(ISqlConnectionFactory connectionFactory) : IGamificationQueries
{
    private const string FirstPageSql = """
        SELECT
            id AS Id,
            event_type AS EventType,
            xp_delta AS XpDelta,
            xp_balance_after AS XpBalanceAfter,
            honor_delta AS HonorDelta,
            honor_balance_after AS HonorBalanceAfter,
            description AS Description,
            reference_entity_type AS ReferenceEntityType,
            reference_entity_id AS ReferenceEntityId,
            created_at_utc AS CreatedAtUtc
        FROM reputation_ledger
        WHERE user_id = @UserId
        ORDER BY id DESC
        LIMIT @FetchLimit;
        """;

    private const string NextPageSql = """
        SELECT
            id AS Id,
            event_type AS EventType,
            xp_delta AS XpDelta,
            xp_balance_after AS XpBalanceAfter,
            honor_delta AS HonorDelta,
            honor_balance_after AS HonorBalanceAfter,
            description AS Description,
            reference_entity_type AS ReferenceEntityType,
            reference_entity_id AS ReferenceEntityId,
            created_at_utc AS CreatedAtUtc
        FROM reputation_ledger
        WHERE user_id = @UserId
          AND id < @CursorId
        ORDER BY id DESC
        LIMIT @FetchLimit;
        """;

    public async Task<CursorPagedResult<ReputationLedgerItemDto>> GetReputationHistoryAsync(
        Guid userId,
        string? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (!CursorCodec.TryDecodeId(cursor, out var cursorId))
            return CursorPagedResult<ReputationLedgerItemDto>.Empty;

        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        var fetchLimit = pageSize + 1;
        var sql = cursorId.HasValue ? NextPageSql : FirstPageSql;

        var command = new CommandDefinition(
            sql,
            new { UserId = userId, CursorId = cursorId, FetchLimit = fetchLimit },
            cancellationToken: cancellationToken);

        var rows = await connection.QueryAsync<ReputationLedgerRow>(command);

        var items = rows
            .Select(r => new ReputationLedgerItemDto(
                r.Id,
                r.EventType,
                r.XpDelta,
                r.XpBalanceAfter,
                r.HonorDelta,
                r.HonorBalanceAfter,
                r.Description,
                r.ReferenceEntityType,
                r.ReferenceEntityId,
                r.CreatedAtUtc))
            .ToList();

        return CursorPagedResult<ReputationLedgerItemDto>.Create(
            items,
            pageSize,
            item => CursorCodec.EncodeId(item.Id));
    }

    private sealed class ReputationLedgerRow
    {
        public Guid Id { get; init; }
        public ReputationEventType EventType { get; init; }
        public int XpDelta { get; init; }
        public int XpBalanceAfter { get; init; }
        public int HonorDelta { get; init; }
        public int HonorBalanceAfter { get; init; }
        public string? Description { get; init; }
        public string? ReferenceEntityType { get; init; }
        public Guid? ReferenceEntityId { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}