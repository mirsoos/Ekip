using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.Subscription;

public sealed record PlanEntitlements
{
    [JsonInclude]
    public int MaxActiveEkips { get; private init; }

    [JsonInclude]
    public int DailyAiMessagesQuota { get; private init; }

    [JsonInclude]
    public bool CanCreateRecurringEkips { get; private init; }

    [JsonInclude]
    public bool CanCreateBusinessEkips { get; private init; }

    [JsonInclude]
    public IReadOnlyDictionary<string, string>? CustomPerks { get; private init; }

    private PlanEntitlements() { }

    private PlanEntitlements(
        int maxActiveEkips,
        int dailyAiMessagesQuota,
        bool canCreateRecurringEkips,
        bool canCreateBusinessEkips,
        IReadOnlyDictionary<string, string>? customPerks)
    {
        MaxActiveEkips = maxActiveEkips;
        DailyAiMessagesQuota = dailyAiMessagesQuota;
        CanCreateRecurringEkips = canCreateRecurringEkips;
        CanCreateBusinessEkips = canCreateBusinessEkips;
        CustomPerks = customPerks;
    }

    public static Result<PlanEntitlements> Create(
        int maxActiveEkips,
        int dailyAiMessagesQuota,
        bool canCreateRecurringEkips,
        bool canCreateBusinessEkips,
        IReadOnlyDictionary<string, string>? customPerks = null)
    {
        if (maxActiveEkips < 0 || dailyAiMessagesQuota < 0)
            return SubscriptionErrors.InvalidEntitlements;

        IReadOnlyDictionary<string, string>? frozenPerks = null;
        if (customPerks is { Count: > 0 })
        {
            var dict = new Dictionary<string, string>(customPerks.Count, StringComparer.Ordinal);
            foreach (var (key, value) in customPerks)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    return SubscriptionErrors.InvalidEntitlements;

                dict[key.Trim()] = value.Trim();
            }

            frozenPerks = new ReadOnlyDictionary<string, string>(dict);
        }

        return new PlanEntitlements(
            maxActiveEkips,
            dailyAiMessagesQuota,
            canCreateRecurringEkips,
            canCreateBusinessEkips,
            frozenPerks);
    }
}