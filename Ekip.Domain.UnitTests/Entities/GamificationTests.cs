using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Events.Gamification;
using Ekip.Domain.Policies;
using Ekip.Domain.UnitTests.Helpers;

namespace Ekip.Domain.UnitTests.Entities;

public sealed class GamificationTests
{
    private static readonly DateTime UtcNow = new(2026, 11, 14, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(0, 1, 100)]
    [InlineData(99, 1, 100)]
    [InlineData(100, 2, 300)]
    [InlineData(299, 2, 300)]
    [InlineData(300, 3, 600)]
    [InlineData(600, 4, 1000)]
    public void ReputationPolicy_Level_Boundaries_Should_Be_Exact(int totalXp, int expectedLevel, int expectedNextXp)
    {
        var level = ReputationPolicy.CalculateLevel(totalXp);
        var nextXp = ReputationPolicy.GetNextLevelXp(level);

        Assert.Equal(expectedLevel, level);
        Assert.Equal(expectedNextXp, nextXp);
    }

    [Fact]
    public void ReputationPolicy_ManualAdjustment_Should_Require_Explicit_Deltas()
    {
        var result = ReputationPolicy.GetReward(ReputationEventType.AdminManualAdjustment);

        ResultAssert.Failure(result, GamificationErrors.ManualAdjustmentRequiresExplicitDeltas);
    }

    [Fact]
    public void Summary_ApplyReputationEvent_Should_LevelUp_Increment_Counters_And_Emit_Events()
    {
        var summary = UserGamificationSummary.Create(Guid.CreateVersion7());

        ResultAssert.Success(summary.ApplyReputationEvent(ReputationEventType.EkipCompleted, 120, 10, UtcNow));
        ResultAssert.Success(summary.ApplyReputationEvent(ReputationEventType.EkipCompleted, 40, 5, UtcNow));

        Assert.Equal(160, summary.TotalXp);
        Assert.Equal(2, summary.CurrentLevel);
        Assert.Equal(300, summary.NextLevelXp);
        Assert.Equal(15, summary.TotalHonor);

        var counter = Assert.Single(summary.AggregatedCounters);
        Assert.Equal(ReputationEventType.EkipCompleted, counter.EventType);
        Assert.Equal(2, counter.Count);

        var levelEvent = ResultAssert.HasDomainEvent<UserLeveledUpDomainEvent>(summary);
        Assert.Equal(1, levelEvent.OldLevel);
        Assert.Equal(2, levelEvent.NewLevel);

        ResultAssert.HasDomainEvent<UserReputationChangedDomainEvent>(summary);
    }

    [Fact]
    public void Summary_Negative_Xp_Should_Floor_At_Zero_While_Honor_Can_Be_Negative()
    {
        var summary = UserGamificationSummary.Create(Guid.CreateVersion7());

        ResultAssert.Success(summary.ApplyReputationEvent(ReputationEventType.NoShowPenalty, -30, -25, UtcNow));

        Assert.Equal(0, summary.TotalXp);
        Assert.Equal(1, summary.CurrentLevel);
        Assert.Equal(-25, summary.TotalHonor);
    }

    [Fact]
    public void Summary_RecordRating_And_RevertRating_Should_Calculate_Exact_Average()
    {
        var summary = UserGamificationSummary.Create(Guid.CreateVersion7());

        ResultAssert.Success(summary.RecordRating(5));
        ResultAssert.Success(summary.RecordRating(4));
        Assert.Equal(4.50m, summary.PublicScore);
        Assert.Equal(2, summary.RatingsCount);

        ResultAssert.Success(summary.RevertRating(4));
        Assert.Equal(5.00m, summary.PublicScore);
        Assert.Equal(1, summary.RatingsCount);
    }

    [Fact]
    public void ReputationLedger_Create_Should_Reject_SelfActor_On_Peer_Event()
    {
        var userId = Guid.CreateVersion7();

        var result = ReputationLedger.Create(
            userId,
            ReputationEventType.UserRated,
            10,
            10,
            2,
            2,
            Guid.CreateVersion7(),
            actorId: userId);

        ResultAssert.Failure(result, GamificationErrors.SelfActorNotAllowed);
    }
}