using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Trend
{
    public class SearchQueryLog : BaseEntity,IAggregateRoot, ICreatedAuditable
    {
        public Guid? UserId { get; private set; }
        public Guid CityId { get; private set; }
        public string NormalizedQuery { get; private set; }
        public int ResultsCount { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
