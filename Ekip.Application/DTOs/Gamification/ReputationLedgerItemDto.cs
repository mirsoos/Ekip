using Ekip.Domain.Enums.Gamification;

namespace Ekip.Application.DTOs.Gamification;

public sealed record ReputationLedgerItemDto(
    Guid Id,
    ReputationEventType EventType,
    int XpDelta,
    int XpBalanceAfter,
    int HonorDelta,
    int HonorBalanceAfter,
    string? Description,
    string? ReferenceEntityType,
    Guid? ReferenceEntityId,
    DateTime CreatedAtUtc);