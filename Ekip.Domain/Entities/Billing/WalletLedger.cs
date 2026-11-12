using Ekip.Domain.Base;
using Ekip.Domain.Enums.Billing;

namespace Ekip.Domain.Entities.Billing;

public class WalletLedger : BaseEntity, IAggregateRoot, ICreatedAuditable
{
    public Guid IdempotencyKey { get; private set; }
    public Guid UserId { get; private set; }
    public WalletTransactionType Type { get; private set; }
    public decimal AmountDelta { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public string Currency { get; private set; }
    public Guid? ReferenceEntityId { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}