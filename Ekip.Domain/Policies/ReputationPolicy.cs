using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Results;

namespace Ekip.Domain.Policies;

public readonly record struct ReputationDelta(int XpDelta, int HonorDelta);

public static class ReputationPolicy
{
    public static Result<ReputationDelta> GetReward(ReputationEventType eventType) => eventType switch
    {
        ReputationEventType.EkipCreated => new ReputationDelta(15, 2),
        ReputationEventType.EkipCompleted => new ReputationDelta(40, 5),
        ReputationEventType.HostCanceledLate => new ReputationDelta(-20, -15),
        ReputationEventType.LeftEkip => new ReputationDelta(0, -3),
        ReputationEventType.KickedFromEkip => new ReputationDelta(-15, -10),
        ReputationEventType.NoShowPenalty => new ReputationDelta(-30, -25),
        ReputationEventType.UserRated => new ReputationDelta(10, 2),
        ReputationEventType.UnwantedCompanionInvite => new ReputationDelta(-10, -10),
        ReputationEventType.BlockedByUser => new ReputationDelta(0, -5),
        ReputationEventType.ReportConfirmed => new ReputationDelta(-50, -30),
        ReputationEventType.NsfwViolation => new ReputationDelta(-100, -50),
        ReputationEventType.FaceVerified => new ReputationDelta(50, 15),
        ReputationEventType.NationalIdVerified => new ReputationDelta(100, 25),
        ReputationEventType.ReferralBonus => new ReputationDelta(30, 5),
        ReputationEventType.MissionCompleted => new ReputationDelta(25, 3),
        ReputationEventType.SubscriptionPurchased => new ReputationDelta(50, 10),
        ReputationEventType.AdminManualAdjustment => GamificationErrors.ManualAdjustmentRequiresExplicitDeltas,
        _ => GamificationErrors.InvalidEventType
    };

    public static Result<ReputationDelta> GetRatingReward(int score) => score switch
    {
        1 => new ReputationDelta(0, -5),
        2 => new ReputationDelta(0, -2),
        3 => new ReputationDelta(5, 0),
        4 => new ReputationDelta(10, 2),
        5 => new ReputationDelta(20, 5),
        _ => GamificationErrors.InvalidRatingScore
    };

    public static int CalculateLevel(int totalXp)
    {
        if (totalXp <= 0)
            return 1;

        var estimated = (int)((1.0 + Math.Sqrt(1.0 + (4.0 * totalXp / 50.0))) / 2.0);
        var level = Math.Max(1, estimated);

        while (level > 1 && totalXp < GetThresholdForLevel(level))
            level--;

        while (totalXp >= GetThresholdForLevel(level + 1))
            level++;

        return level;
    }

    public static int GetNextLevelXp(int currentLevel)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(currentLevel, 1);
        var threshold = GetThresholdForLevel(currentLevel + 1);
        return (int)Math.Min(int.MaxValue, threshold);
    }

    private static long GetThresholdForLevel(int level) => 50L * level * (level - 1);
}