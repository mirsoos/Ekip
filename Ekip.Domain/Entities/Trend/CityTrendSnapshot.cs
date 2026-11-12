using Ekip.Domain.Base;
using Ekip.Domain.Enums.Trend;

namespace Ekip.Domain.Entities.Trend
{
    public class CityTrendSnapshot : BaseEntity, IAggregateRoot, ICreatedAuditable
    {
        public Guid CityId { get; private set; }
        public TrendType Type { get; private set; }
        public string Keyword { get; private set; }
        public decimal TrendScore { get; private set; }
        public int ActiveEkipsCount { get; private set; }
        public int Rank { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
