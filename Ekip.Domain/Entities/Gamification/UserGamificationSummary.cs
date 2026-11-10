using Ekip.Domain.Base;
using Ekip.Domain.ValueObjects.Gamification;

namespace Ekip.Domain.Entities.Gamification
{
    public class UserGamificationSummary : Entity , IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public int TotalXp { get; private set; }
        public int CurrentLevel { get; private set; } = 1;
        public decimal PublicScore { get; private set; }
        public int RatingScoresSum { get; private set; }
        public int NextLevelXp { get; private set; }
        public int RatingsCount { get; private set; }
        public int TotalHonor { get; private set; } = 0;
        private readonly List<AggregatedCounters> _aggregatedCounters = [];
        public IReadOnlyCollection<AggregatedCounters> AggregatedCounters => _aggregatedCounters.AsReadOnly();
    }
}
