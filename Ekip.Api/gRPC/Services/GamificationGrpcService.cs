using Ekip.Api.Grpc.Gamification;
using Ekip.Application.Features.Gamification.Queries.GetReputationHistory;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Ekip.Api.gRPC.Services;

public sealed class GamificationGrpcService(ISender sender) : GamificationGrpc.GamificationGrpcBase
{
    public override async Task<GetReputationHistoryGrpcResponse> GetReputationHistory(
        GetReputationHistoryGrpcRequest request,
        ServerCallContext context)
    {
        var userId = Guid.TryParse(request.UserId, out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        var pageSize = request.PageSize == 0 ? 20 : request.PageSize;
        var cursor = request.HasCursor ? request.Cursor : null;

        var query = new GetReputationHistoryQuery(userId, cursor, pageSize);
        var result = await sender.Send(query, context.CancellationToken);
        var page = result.GetValueOrThrowRpc();

        var response = new GetReputationHistoryGrpcResponse
        {
            HasMore = page.HasMore
        };

        if (page.NextCursor is not null)
            response.NextCursor = page.NextCursor;

        foreach (var item in page.Items)
        {
            var grpcItem = new ReputationLedgerItemGrpcDto
            {
                Id = item.Id.ToString("D"),
                EventType = (int)item.EventType,
                XpDelta = item.XpDelta,
                XpBalanceAfter = item.XpBalanceAfter,
                HonorDelta = item.HonorDelta,
                HonorBalanceAfter = item.HonorBalanceAfter,
                CreatedAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(item.CreatedAtUtc, DateTimeKind.Utc))
            };

            if (item.Description is not null)
                grpcItem.Description = item.Description;

            if (item.ReferenceEntityType is not null)
                grpcItem.ReferenceEntityType = item.ReferenceEntityType;

            if (item.ReferenceEntityId.HasValue)
                grpcItem.ReferenceEntityId = item.ReferenceEntityId.Value.ToString("D");

            response.Items.Add(grpcItem);
        }

        return response;
    }
}