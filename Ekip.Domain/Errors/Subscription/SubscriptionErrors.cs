using Ekip.Domain.Results;

namespace Ekip.Domain.Errors.Subscription;

public static class SubscriptionErrors
{
    public static readonly Error InvalidPlanCode = Error.Validation(
        "SUBSCRIPTION.INVALID_PLAN_CODE",
        "Plan code is required and must be uppercase alphanumeric/underscore up to 50 characters.");

    public static readonly Error InvalidPlanTitle = Error.Validation(
        "SUBSCRIPTION.INVALID_PLAN_TITLE",
        "Plan title is required and cannot exceed 150 characters.");

    public static readonly Error InvalidDuration = Error.Validation(
        "SUBSCRIPTION.INVALID_DURATION",
        "Plan duration must be greater than zero for paid plans or non-negative for free plans.");

    public static readonly Error InvalidPriceAmount = Error.Validation(
        "SUBSCRIPTION.INVALID_PRICE_AMOUNT",
        "Subscription price amount cannot be negative.");

    public static readonly Error InvalidCurrency = Error.Validation(
        "SUBSCRIPTION.INVALID_CURRENCY",
        "Currency code must be a valid 3-to-10 character ISO code.");

    public static readonly Error InvalidEntitlements = Error.Validation(
        "SUBSCRIPTION.INVALID_ENTITLEMENTS",
        "Entitlement limits cannot be negative.");

    public static readonly Error InvalidQuotaValues = Error.Validation(
        "SUBSCRIPTION.INVALID_QUOTA_VALUES",
        "Quota used and limit cannot be negative, and used cannot exceed limit.");

    public static readonly Error PlanNotActive = Error.Conflict(
        "SUBSCRIPTION.PLAN_NOT_ACTIVE",
        "Cannot subscribe to an inactive subscription plan.");

    public static readonly Error SubscriptionExpired = Error.Forbidden(
        "SUBSCRIPTION.EXPIRED",
        "User subscription has expired.");

    public static readonly Error DailyAiQuotaExceeded = Error.Forbidden(
        "SUBSCRIPTION.DAILY_AI_QUOTA_EXCEEDED",
        "Daily AI messages quota has been exhausted.");
}