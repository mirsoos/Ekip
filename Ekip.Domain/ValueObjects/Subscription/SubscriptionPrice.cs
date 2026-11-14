using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.Subscription;

public sealed record SubscriptionPrice
{
    public decimal Amount { get; private init; }
    public string Currency { get; private init; } = string.Empty;

    private SubscriptionPrice() { }

    private SubscriptionPrice(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency;
    }

    public static Result<SubscriptionPrice> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return SubscriptionErrors.InvalidPriceAmount;

        if (string.IsNullOrWhiteSpace(currency))
            return SubscriptionErrors.InvalidCurrency;

        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        if (normalizedCurrency.Length is < 3 or > 10)
            return SubscriptionErrors.InvalidCurrency;

        return new SubscriptionPrice(amount, normalizedCurrency);
    }

    public static SubscriptionPrice Free(string currency = "IRR") =>
        new(0m, currency.Trim().ToUpperInvariant());
}