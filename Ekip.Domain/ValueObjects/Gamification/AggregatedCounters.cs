using Ekip.Domain.Enums.Gamification;

namespace Ekip.Domain.ValueObjects.Gamification;
public sealed record AggregatedCounters(
    ReputationEventType EventType,
    int Count);