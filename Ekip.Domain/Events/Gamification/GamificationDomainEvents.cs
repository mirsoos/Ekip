using Ekip.Domain.Base;

namespace Ekip.Domain.Events.Gamification;

public sealed record UserLeveledUpDomainEvent(
    Guid UserId,
    int OldLevel,
    int NewLevel,
    int TotalXp,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);

public sealed record UserReputationChangedDomainEvent(
    Guid UserId,
    int TotalXp,
    int CurrentLevel,
    int TotalHonor,
    DateTime OccurredOnUtc) : DomainEventBase(Guid.CreateVersion7(), OccurredOnUtc);