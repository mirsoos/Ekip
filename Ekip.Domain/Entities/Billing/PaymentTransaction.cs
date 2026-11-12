using Ekip.Domain.Base;
using Ekip.Domain.Enums.Billing;

namespace Ekip.Domain.Entities.Billing;

public class PaymentTransaction : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public Guid IdempotencyKey { get; private set; }
    public Guid UserId { get; private set; }
    public PaymentPurpose Purpose { get; private set; }
    public Guid ReferenceEntityId { get; private set; }
    public Guid? PromoCodeId { get; private set; }
    public decimal BaseAmount { get; private set; }
    public decimal DiscountAmount { get; private set; } = 0;
    public decimal WalletAmountUsed { get; private set; } = 0;
    public decimal GatewayPayableAmount { get; private set; }
    public string Currency { get; private set; }
    public PaymentGateway Gateway { get; private set; }
    public string? AuthorityOrToken { get; private set; }
    public string? ReferenceId { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? FailureReason { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? RefundedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
}