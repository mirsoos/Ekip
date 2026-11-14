using Ekip.Domain.Base;
using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Results;

namespace Ekip.Domain.Entities.Gamification;

public class ReputationLedger : BaseEntity, IAggregateRoot, ICreatedAuditable, IAppendOnlyEntity
{
    public Guid UserId { get; private set; }
    public Guid? ActorId { get; private set; }
    public string? Description { get; private set; }
    public ReputationEventType EventType { get; private set; }
    public int XpDelta { get; private set; }
    public int XpBalanceAfter { get; private set; }
    public int HonorDelta { get; private set; }
    public int HonorBalanceAfter { get; private set; }
    public string? ReferenceEntityType { get; private set; }
    public Guid? ReferenceEntityId { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ReputationLedger() { }

    private ReputationLedger(
        Guid userId,
        ReputationEventType eventType,
        int xpDelta,
        int xpBalanceAfter,
        int honorDelta,
        int honorBalanceAfter,
        Guid idempotencyKey,
        Guid? actorId,
        string? description,
        string? referenceEntityType,
        Guid? referenceEntityId)
    {
        UserId = userId;
        EventType = eventType;
        XpDelta = xpDelta;
        XpBalanceAfter = xpBalanceAfter;
        HonorDelta = honorDelta;
        HonorBalanceAfter = honorBalanceAfter;
        IdempotencyKey = idempotencyKey;
        ActorId = actorId;
        Description = description;
        ReferenceEntityType = referenceEntityType;
        ReferenceEntityId = referenceEntityId;
    }

    public static Result<ReputationLedger> Create(
        Guid userId,
        ReputationEventType eventType,
        int xpDelta,
        int xpBalanceAfter,
        int honorDelta,
        int honorBalanceAfter,
        Guid idempotencyKey,
        Guid? actorId = null,
        string? description = null,
        string? referenceEntityType = null,
        Guid? referenceEntityId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (idempotencyKey == Guid.Empty)
            throw new ArgumentException("IdempotencyKey cannot be empty.", nameof(idempotencyKey));
        if (actorId == Guid.Empty)
            throw new ArgumentException("ActorId cannot be empty when provided.", nameof(actorId));
        if (referenceEntityId == Guid.Empty)
            throw new ArgumentException("ReferenceEntityId cannot be empty when provided.", nameof(referenceEntityId));

        if (!Enum.IsDefined(eventType))
            return GamificationErrors.InvalidEventType;

        if (xpDelta == 0 && honorDelta == 0)
            return GamificationErrors.InvalidReputationDelta;

        if (xpBalanceAfter < 0)
            return GamificationErrors.InvalidBalance;

        if (actorId.HasValue && actorId.Value == userId && IsPeerEvent(eventType))
            return GamificationErrors.SelfActorNotAllowed;

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription?.Length > 500)
            return GamificationErrors.InvalidDescription;

        var normalizedRefType = string.IsNullOrWhiteSpace(referenceEntityType) ? null : referenceEntityType.Trim();
        if (normalizedRefType?.Length > 100)
            return GamificationErrors.InvalidReferenceEntityType;

        return new ReputationLedger(
            userId,
            eventType,
            xpDelta,
            xpBalanceAfter,
            honorDelta,
            honorBalanceAfter,
            idempotencyKey,
            actorId,
            normalizedDescription,
            normalizedRefType,
            referenceEntityId);
    }

    private static bool IsPeerEvent(ReputationEventType eventType) => eventType is
        ReputationEventType.UserRated
        or ReputationEventType.ReferralBonus
        or ReputationEventType.BlockedByUser
        or ReputationEventType.UnwantedCompanionInvite;
}