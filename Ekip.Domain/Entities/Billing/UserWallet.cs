using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Billing;

public class UserWallet : Entity, IAggregateRoot, IAuditableEntity
{
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; } = 0;
    public decimal LockedBalance { get; private set; } = 0;
    public string Currency { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public decimal AvailableBalance => Balance - LockedBalance;
}