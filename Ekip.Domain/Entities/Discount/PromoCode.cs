using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Discount
{
    
    public class PromoCode : BaseEntity,IAggregateRoot, IAuditableEntity
    {
        public string Code { get; private set; }
        public int DiscountPercent { get; private set; }
        public decimal? MaxDiscountAmount { get; private set; }
        public int MaxUses { get; private set; }
        public int UsedCount { get; private set; } = 0;
        public Guid? TargetUserId { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsCurrentlyValid => IsActive && UsedCount < MaxUses && ExpiresAtUtc > DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
    }
}
