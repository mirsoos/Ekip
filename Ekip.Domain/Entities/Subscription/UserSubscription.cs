using Ekip.Domain.Base;
using Ekip.Domain.ValueObjects.Subscription;
namespace Ekip.Domain.Entities.Subscription
{
    public class UserSubscription : Entity, IAuditableEntity
    {
        public Guid UserId { get; private set; }
        public Guid SubscriptionPlanId { get; private set; }
        public DateTime StartsAtUtc { get; private set; }
        public DateTime? ExpiresAtUtc { get; private set; }
        public QuotaUsage QuotaUsage { get; private set; }
        public Guid IdempotencyKey { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public bool IsActiveAt(DateTime utcNow) => ExpiresAtUtc is null || ExpiresAtUtc > utcNow;
    }
}
