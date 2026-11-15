using Ekip.Application.Contracts;
using Ekip.Application.DTOs.Gamification;
using Ekip.Application.Interfaces.Queries.Gamification;
using Ekip.Domain.Results;

namespace Ekip.Application.Features.Gamification.Queries.GetReputationHistory;

internal sealed class GetReputationHistoryQueryHandler(
    IGamificationQueries gamificationQueries)
    : IQueryHandler<GetReputationHistoryQuery, CursorPagedResult<ReputationLedgerItemDto>>
{
    public async Task<Result<CursorPagedResult<ReputationLedgerItemDto>>> Handle(
        GetReputationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var page = await gamificationQueries.GetReputationHistoryAsync(
            request.UserId,
            request.Cursor,
            request.PageSize,
            cancellationToken);

        return page;
    }
}