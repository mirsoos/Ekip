namespace Ekip.Domain.ValueObjects.Subscription;
public sealed record PlanEntitlements(
    int MaxActiveEkips,
    int DailyAiMessagesQuota,
    bool CanCreateRecurringEkips,
    bool CanCreateBusinessEkips,
    IReadOnlyDictionary<string, string>? CustomPerks = null
);