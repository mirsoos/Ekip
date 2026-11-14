using Ekip.Domain.Base;
using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.Events.Subscription;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.Subscription;

namespace Ekip.Domain.Entities.Subscription;

public class UserSubscription : Entity, IAggregateRoot, IAuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public QuotaUsage QuotaUsage { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private UserSubscription()
    {
        QuotaUsage = null!;
    }

    private UserSubscription(
        Guid userId,
        Guid subscriptionPlanId,
        DateTime startsAtUtc,
        DateTime? expiresAtUtc,
        QuotaUsage quotaUsage,
        Guid idempotencyKey)
    {
        UserId = userId;
        SubscriptionPlanId = subscriptionPlanId;
        StartsAtUtc = startsAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        QuotaUsage = quotaUsage;
        IdempotencyKey = idempotencyKey;
    }

    public bool IsActiveAt(DateTime utcNow) => ExpiresAtUtc is null || ExpiresAtUtc > utcNow;

    public static Result<UserSubscription> Create(
        Guid userId,
        SubscriptionPlan plan,
        Guid idempotencyKey,
        DateTime utcNow)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (idempotencyKey == Guid.Empty)
            throw new ArgumentException("IdempotencyKey cannot be empty.", nameof(idempotencyKey));
        ArgumentNullException.ThrowIfNull(plan);

        if (!plan.IsActive)
            return SubscriptionErrors.PlanNotActive;

        DateTime? expiresAtUtc = plan.DurationDays > 0
            ? utcNow.AddDays(plan.DurationDays)
            : null;

        var quota = QuotaUsage.Initial(plan.Entitlements.DailyAiMessagesQuota, utcNow);

        var subscription = new UserSubscription(
            userId,
            plan.Id,
            utcNow,
            expiresAtUtc,
            quota,
            idempotencyKey);

        subscription.AddDomainEvent(new SubscriptionActivatedDomainEvent(
            userId,
            plan.Id,
            expiresAtUtc,
            utcNow));

        return subscription;
    }

    public Result ApplyPlanChange(
        SubscriptionPlan newPlan,
        Guid idempotencyKey,
        DateTime utcNow)
    {
        if (idempotencyKey == Guid.Empty)
            throw new ArgumentException("IdempotencyKey cannot be empty.", nameof(idempotencyKey));
        ArgumentNullException.ThrowIfNull(newPlan);

        if (IdempotencyKey == idempotencyKey)
            return Result.Success();

        if (!newPlan.IsActive)
            return SubscriptionErrors.PlanNotActive;

        var isActiveSamePaidPlan =
            SubscriptionPlanId == newPlan.Id
            && newPlan.DurationDays > 0
            && ExpiresAtUtc is not null
            && ExpiresAtUtc > utcNow;

        if (isActiveSamePaidPlan)
        {
            ExpiresAtUtc = ExpiresAtUtc!.Value.AddDays(newPlan.DurationDays);
        }
        else
        {
            StartsAtUtc = utcNow;
            ExpiresAtUtc = newPlan.DurationDays > 0
                ? utcNow.AddDays(newPlan.DurationDays)
                : null;
        }

        var newLimit = newPlan.Entitlements.DailyAiMessagesQuota;
        var needsReset = QuotaUsage.ResetAtUtc is null || utcNow >= QuotaUsage.ResetAtUtc.Value;
        var currentUsed = needsReset ? 0 : Math.Min(QuotaUsage.Used, newLimit);
        var nextReset = needsReset ? utcNow.Date.AddDays(1) : QuotaUsage.ResetAtUtc;

        QuotaUsage = QuotaUsage.Create(currentUsed, newLimit, nextReset).Value;
        SubscriptionPlanId = newPlan.Id;
        IdempotencyKey = idempotencyKey;

        AddDomainEvent(new SubscriptionActivatedDomainEvent(
            UserId,
            SubscriptionPlanId,
            ExpiresAtUtc,
            utcNow));

        return Result.Success();
    }

    public Result ConsumeAiMessageQuota(int planDailyLimit, DateTime utcNow)
    {
        if (!IsActiveAt(utcNow))
            return SubscriptionErrors.SubscriptionExpired;

        var result = QuotaUsage.ConsumeOne(planDailyLimit, utcNow);
        if (result.IsFailure)
            return result.Error;

        QuotaUsage = result.Value;
        return Result.Success();
    }
}