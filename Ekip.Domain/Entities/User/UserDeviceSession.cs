using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;

namespace Ekip.Domain.Entities.User;

public class UserDeviceSession : BaseEntity, IAggregateRoot, IAuditableEntity
{
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; }
    public DeviceType DeviceType { get; private set; }
    public string? DeviceName { get; private set; }
    public string? AppVersion { get; private set; }
    public string? FcmPushToken { get; private set; }
    public string RefreshTokenHash { get; private set; }
    public int TokenVersion { get; private set; } = 1;
    public string? IpAddress { get; private set; }
    public DateTime LastActiveAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTime? RevokedAtUtc { get; private set; }
    public string? RevokedReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public bool IsActiveAt(DateTime utcNow) => !IsRevoked && ExpiresAtUtc > utcNow;
}