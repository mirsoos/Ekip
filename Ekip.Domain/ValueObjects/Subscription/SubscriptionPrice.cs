namespace Ekip.Domain.ValueObjects.Subscription;
public sealed record SubscriptionPrice(
    decimal Amount,
    string Currency
);