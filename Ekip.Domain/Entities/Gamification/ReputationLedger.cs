using Ekip.Domain.Base;
using Ekip.Domain.Enums.Gamification;

namespace Ekip.Domain.Entities.Gamification
{
    public class ReputationLedger : BaseEntity, IAggregateRoot, ICreatedAuditable
    {
        public Guid UserId { get; private set; }
        public Guid? ActorId { get; private set; }
        public string? Description { get; private set; }
        public ReputationEventType EventType { get; private set; }
        public int XpDelta { get; private set; }
        public int XpBalanceAfter { get; private set; }
        public int HonorDelta { get; private set; }
        public int HonorBalanceAfter { get; private set; }
        public string? ReferenceEntityType { get; private set; }
        public Guid? ReferenceEntityId { get; private set; }
        public Guid IdempotencyKey { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
