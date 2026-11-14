using Ekip.Domain.Base;

namespace Ekip.Domain.Events.Subscription;

public sealed record SubscriptionActivatedDomainEvent(
    Guid UserId,
    Guid SubscriptionPlanId,
    DateTime? ExpiresAtUtc,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);