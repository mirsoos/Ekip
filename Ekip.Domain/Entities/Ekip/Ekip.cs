using Ekip.Domain.Base;
using Ekip.Domain.Enums.Ekip;
using Ekip.Domain.ValueObjects.Ekip;

namespace Ekip.Domain.Entities.Ekip
{
    public class Ekip : BaseEntity , IAggregateRoot, IAuditableEntity, ISoftDeletable
    {
        public Guid IdempotencyKey { get; private set; }
        public Guid CreatorId { get; private set; }
        public Guid? RecurringEkipId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public EkipCategory Category { get; private set; }
        public string SubCategory { get; private set; }
        public Guid CityId { get; private set; }
        public Guid? SponsoredPlaceId { get; private set; }
        public GeoLocation ExactLocationPoint { get; private set; }
        public GeoLocation PublicMapPoint { get; private set; }
        public string LocationName { get; private set; }
        public string AddressDetail { get; private set; }
        public bool IsDetailsPrivate { get; private set; } = false;
        public EkipComposition HostComposition { get; private set; }
        public int MinMembers { get; private set; }
        public int? MaxMembers { get; private set; }
        public int ApprovedMembersCount { get; private set; } = 1;
        public decimal? EstimatedCost { get; private set; }
        public DateTime EventDateTimeUtc { get; private set; }
        public DateTime JoinDeadlineUtc { get; private set; }
        public EkipJoinCriteria JoinCriteria { get; private set; }
        public int ViewsCount { get; private set; } = 0;
        public EkipModerationStatus ModerationStatus { get; private set; } = EkipModerationStatus.PendingByAI;
        public EkipLifecycleStatus LifecycleStatus { get; private set; }
        public DateTime? CanceledAtUtc { get; private set; }
        public string? CancellationReason { get; private set; }
        public QuorumStatus QuorumStatus { get; private set; } = QuorumStatus.None;
        public DateTime? QuorumDecisionDeadlineUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
        private readonly List<string> _tags = [];
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
        private readonly List<Guid> _mediaAssetIds = [];
        public IReadOnlyCollection<Guid> MediaAssetIds => _mediaAssetIds.AsReadOnly();
    }
}
