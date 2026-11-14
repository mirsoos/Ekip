using Ekip.Domain.Errors.Subscription;
using Ekip.Domain.Results;

namespace Ekip.Domain.ValueObjects.Subscription;

public sealed record QuotaUsage
{
    public int Used { get; private init; }
    public int Limit { get; private init; }
    public DateTime? ResetAtUtc { get; private init; }

    private QuotaUsage() { }

    private QuotaUsage(int used, int limit, DateTime? resetAtUtc)
    {
        Used = used;
        Limit = limit;
        ResetAtUtc = resetAtUtc;
    }

    public static Result<QuotaUsage> Create(int used, int limit, DateTime? resetAtUtc = null)
    {
        if (used < 0 || limit < 0 || used > limit)
            return SubscriptionErrors.InvalidQuotaValues;

        return new QuotaUsage(used, limit, resetAtUtc);
    }

    public static QuotaUsage Initial(int dailyLimit, DateTime utcNow)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(dailyLimit);
        var nextResetUtc = utcNow.Date.AddDays(1);
        return new QuotaUsage(0, dailyLimit, nextResetUtc);
    }

    public Result<QuotaUsage> ConsumeOne(int currentPlanLimit, DateTime utcNow)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(currentPlanLimit);

        var needsReset = ResetAtUtc is null || utcNow >= ResetAtUtc.Value;
        var currentUsed = needsReset ? 0 : Used;
        var nextReset = needsReset ? utcNow.Date.AddDays(1) : ResetAtUtc;

        if (currentUsed >= currentPlanLimit)
            return SubscriptionErrors.DailyAiQuotaExceeded;

        return new QuotaUsage(currentUsed + 1, currentPlanLimit, nextReset);
    }
}