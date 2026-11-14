using Ekip.Domain.Entities.Subscription;
using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.UnitTests.Helpers;
using Ekip.Domain.ValueObjects.Subscription;

namespace Ekip.Domain.UnitTests.Entities;

public sealed class SubscriptionTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    private static SubscriptionPlan CreatePlan(string code, decimal amount, int durationDays, int aiQuota, bool isActive = true)
    {
        var price = ResultAssert.Success(SubscriptionPrice.Create(amount, "IRR"));
        var entitlements = ResultAssert.Success(PlanEntitlements.Create(5, aiQuota, amount > 0, amount > 0));
        return ResultAssert.Success(SubscriptionPlan.Create(code, $"{code} Plan", price, entitlements, durationDays, isActive));
    }

    [Fact]
    public void SubscriptionPlan_Paid_With_Zero_Duration_Should_Fail()
    {
        var price = ResultAssert.Success(SubscriptionPrice.Create(100_000m, "IRR"));
        var entitlements = ResultAssert.Success(PlanEntitlements.Create(5, 20, true, false));

        var result = SubscriptionPlan.Create("PLUS_MONTHLY", "Plus", price, entitlements, 0);

        ResultAssert.Failure(result, SubscriptionErrors.InvalidDuration);
    }

    [Fact]
    public void UserSubscription_ApplyPlanChange_Should_Be_Idempotent_And_Extend_Active_Same_Plan()
    {
        var userId = Guid.CreateVersion7();
        var plusPlan = CreatePlan("PLUS_MONTHLY", 200_000m, 30, 50);
        var firstKey = Guid.CreateVersion7();

        var sub = ResultAssert.Success(UserSubscription.Create(userId, plusPlan, firstKey, UtcNow));
        var expectedFirstExpiry = UtcNow.AddDays(30);
        Assert.Equal(expectedFirstExpiry, sub.ExpiresAtUtc);

        // Same key => idempotent no-op
        ResultAssert.Success(sub.ApplyPlanChange(plusPlan, firstKey, UtcNow.AddDays(1)));
        Assert.Equal(expectedFirstExpiry, sub.ExpiresAtUtc);

        // Renew same active paid plan => extends from existing ExpiresAtUtc
        var renewKey = Guid.CreateVersion7();
        ResultAssert.Success(sub.ApplyPlanChange(plusPlan, renewKey, UtcNow.AddDays(10)));
        Assert.Equal(expectedFirstExpiry.AddDays(30), sub.ExpiresAtUtc);
    }

    [Fact]
    public void UserSubscription_ConsumeAiMessageQuota_Should_Enforce_Limit_And_Reset_Next_Day()
    {
        var freePlan = CreatePlan("FREE_TIER", 0m, 0, 2);
        var sub = ResultAssert.Success(UserSubscription.Create(Guid.CreateVersion7(), freePlan, Guid.CreateVersion7(), UtcNow));

        ResultAssert.Success(sub.ConsumeAiMessageQuota(2, UtcNow));
        ResultAssert.Success(sub.ConsumeAiMessageQuota(2, UtcNow.AddHours(1)));
        ResultAssert.Failure(sub.ConsumeAiMessageQuota(2, UtcNow.AddHours(2)), SubscriptionErrors.DailyAiQuotaExceeded);

        // Next UTC day => resets cleanly
        var nextDay = UtcNow.Date.AddDays(1).AddMinutes(5);
        ResultAssert.Success(sub.ConsumeAiMessageQuota(2, nextDay));
        Assert.Equal(1, sub.QuotaUsage.Used);
    }

    [Fact]
    public void UserSubscription_ConsumeAiMessageQuota_Should_Fail_When_Expired()
    {
        var plusPlan = CreatePlan("PLUS_MONTHLY", 200_000m, 30, 50);
        var sub = ResultAssert.Success(UserSubscription.Create(Guid.CreateVersion7(), plusPlan, Guid.CreateVersion7(), UtcNow));

        var result = sub.ConsumeAiMessageQuota(50, UtcNow.AddDays(31));

        ResultAssert.Failure(result, SubscriptionErrors.SubscriptionExpired);
    }
}