using Ekip.Domain.Base;
using Ekip.Domain.Enums.Gamification;
using Ekip.Domain.Errors.Gamification;
using Ekip.Domain.Events.Gamification;
using Ekip.Domain.Policies;
using Ekip.Domain.Results;
using Ekip.Domain.ValueObjects.Gamification;

namespace Ekip.Domain.Entities.Gamification;

public class UserGamificationSummary : Entity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public int TotalXp { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    public decimal PublicScore { get; private set; }
    public int RatingScoresSum { get; private set; }
    public int NextLevelXp { get; private set; }
    public int RatingsCount { get; private set; }
    public int TotalHonor { get; private set; }

    private readonly List<AggregatedCounters> _aggregatedCounters = [];
    public IReadOnlyCollection<AggregatedCounters> AggregatedCounters => _aggregatedCounters.AsReadOnly();

    private UserGamificationSummary() { }

    private UserGamificationSummary(Guid userId)
    {
        UserId = userId;
        CurrentLevel = 1;
        NextLevelXp = ReputationPolicy.GetNextLevelXp(1);
    }

    public static UserGamificationSummary Create(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        return new UserGamificationSummary(userId);
    }

    public Result ApplyReputationEvent(
        ReputationEventType eventType,
        int xpDelta,
        int honorDelta,
        DateTime utcNow)
    {
        if (!Enum.IsDefined(eventType))
            return GamificationErrors.InvalidEventType;

        if (xpDelta == 0 && honorDelta == 0)
            return GamificationErrors.InvalidReputationDelta;

        var oldLevel = CurrentLevel;
        var oldHonor = TotalHonor;

        var updatedXp = Math.Max(0L, (long)TotalXp + xpDelta);
        TotalXp = (int)Math.Min(int.MaxValue, updatedXp);
        TotalHonor = checked(TotalHonor + honorDelta);

        CurrentLevel = ReputationPolicy.CalculateLevel(TotalXp);
        NextLevelXp = ReputationPolicy.GetNextLevelXp(CurrentLevel);

        var index = _aggregatedCounters.FindIndex(c => c.EventType == eventType);
        if (index >= 0)
            _aggregatedCounters[index] = _aggregatedCounters[index].Increment();
        else
            _aggregatedCounters.Add(ValueObjects.Gamification.AggregatedCounters.Initial(eventType));

        if (CurrentLevel > oldLevel)
            AddDomainEvent(new UserLeveledUpDomainEvent(UserId, oldLevel, CurrentLevel, TotalXp, utcNow));

        if (CurrentLevel != oldLevel || TotalHonor != oldHonor)
            AddDomainEvent(new UserReputationChangedDomainEvent(UserId, TotalXp, CurrentLevel, TotalHonor, utcNow));

        return Result.Success();
    }

    public Result RecordRating(int score)
    {
        if (score is < 1 or > 5)
            return GamificationErrors.InvalidRatingScore;

        RatingScoresSum = checked(RatingScoresSum + score);
        RatingsCount = checked(RatingsCount + 1);
        PublicScore = decimal.Round((decimal)RatingScoresSum / RatingsCount, 2, MidpointRounding.AwayFromZero);

        return Result.Success();
    }

    public Result RevertRating(int score)
    {
        if (score is < 1 or > 5)
            return GamificationErrors.InvalidRatingScore;

        if (RatingsCount <= 0 || RatingScoresSum < score)
            return GamificationErrors.InvalidBalance;

        RatingScoresSum -= score;
        RatingsCount -= 1;
        PublicScore = RatingsCount == 0
            ? 0m
            : decimal.Round((decimal)RatingScoresSum / RatingsCount, 2, MidpointRounding.AwayFromZero);

        return Result.Success();
    }
}