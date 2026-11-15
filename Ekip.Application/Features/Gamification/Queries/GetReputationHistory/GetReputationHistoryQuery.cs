using Ekip.Application.Contracts;
using Ekip.Application.DTOs.Gamification;

namespace Ekip.Application.Features.Gamification.Queries.GetReputationHistory;

public sealed record GetReputationHistoryQuery(
    Guid UserId,
    string? Cursor = null,
    int PageSize = 20) : IQuery<CursorPagedResult<ReputationLedgerItemDto>>;