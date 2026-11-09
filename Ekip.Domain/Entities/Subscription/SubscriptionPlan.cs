using Ekip.Domain.Base;
using Ekip.Domain.ValueObjects.Subscription;

namespace Ekip.Domain.Entities.Subscription
{
    public class SubscriptionPlan : BaseEntity, IAggregateRoot,ICreatedAuditable
    {
        public string Code { get; private set; }
        public string Title { get; private set; }
        public SubscriptionPrice Price { get; private set; }
        public PlanEntitlements Entitlements { get; private set; }
        public int DurationDays { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
