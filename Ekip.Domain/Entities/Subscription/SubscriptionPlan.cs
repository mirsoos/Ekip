using System.Text.RegularExpressions;
using Ekip.Domain.Base;
using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.Subscription;

namespace Ekip.Domain.Entities.Subscription;

public partial class SubscriptionPlan : BaseEntity, IAggregateRoot, ICreatedAuditable
{
    [GeneratedRegex(@"^[A-Z0-9_]{2,50}$", RegexOptions.CultureInvariant)]
    private static partial Regex CodeRegex();

    public string Code { get; private set; }
    public string Title { get; private set; }
    public SubscriptionPrice Price { get; private set; }
    public PlanEntitlements Entitlements { get; private set; }
    public int DurationDays { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SubscriptionPlan()
    {
        Code = null!;
        Title = null!;
        Price = null!;
        Entitlements = null!;
    }

    private SubscriptionPlan(
        string code,
        string title,
        SubscriptionPrice price,
        PlanEntitlements entitlements,
        int durationDays,
        bool isActive)
    {
        Code = code;
        Title = title;
        Price = price;
        Entitlements = entitlements;
        DurationDays = durationDays;
        IsActive = isActive;
    }

    public static Result<SubscriptionPlan> Create(
        string code,
        string title,
        SubscriptionPrice price,
        PlanEntitlements entitlements,
        int durationDays,
        bool isActive = true)
    {
        ArgumentNullException.ThrowIfNull(price);
        ArgumentNullException.ThrowIfNull(entitlements);

        if (string.IsNullOrWhiteSpace(code))
            return SubscriptionErrors.InvalidPlanCode;

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!CodeRegex().IsMatch(normalizedCode))
            return SubscriptionErrors.InvalidPlanCode;

        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 150)
            return SubscriptionErrors.InvalidPlanTitle;

        if ((price.Amount > 0 && durationDays <= 0) || (price.Amount == 0 && durationDays < 0))
            return SubscriptionErrors.InvalidDuration;

        return new SubscriptionPlan(
            normalizedCode,
            title.Trim(),
            price,
            entitlements,
            durationDays,
            isActive);
    }

    public Result UpdateDetails(
        string title,
        SubscriptionPrice price,
        PlanEntitlements entitlements,
        int durationDays)
    {
        ArgumentNullException.ThrowIfNull(price);
        ArgumentNullException.ThrowIfNull(entitlements);

        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 150)
            return SubscriptionErrors.InvalidPlanTitle;

        if ((price.Amount > 0 && durationDays <= 0) || (price.Amount == 0 && durationDays < 0))
            return SubscriptionErrors.InvalidDuration;

        Title = title.Trim();
        Price = price;
        Entitlements = entitlements;
        DurationDays = durationDays;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}