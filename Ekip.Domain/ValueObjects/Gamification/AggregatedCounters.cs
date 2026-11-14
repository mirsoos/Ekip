using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.Gamification;

public sealed record AggregatedCounters
{
    public ReputationEventType EventType { get; private init; }
    public int Count { get; private init; }

    private AggregatedCounters() { }

    private AggregatedCounters(ReputationEventType eventType, int count)
    {
        EventType = eventType;
        Count = count;
    }

    public static Result<AggregatedCounters> Create(ReputationEventType eventType, int count)
    {
        if (!Enum.IsDefined(eventType))
            return GamificationErrors.InvalidEventType;

        if (count < 0)
            return GamificationErrors.InvalidBalance;

        return new AggregatedCounters(eventType, count);
    }

    internal static AggregatedCounters Initial(ReputationEventType eventType) => new(eventType, 1);

    internal AggregatedCounters Increment() => new(EventType, checked(Count + 1));
}