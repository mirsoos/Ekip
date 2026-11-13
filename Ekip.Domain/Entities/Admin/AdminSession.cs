using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Admin
{
    public class AdminSession : BaseEntity , IAggregateRoot, IAuditableEntity
    {
        public Guid AdminId { get; private set; }
        public string RefreshTokenHash { get; private set; }
        public int TokenVersion { get; private set; } = 1;
        public string IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime? RevokedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsActiveAt(DateTime utcNow) => !IsRevoked && ExpiresAtUtc > utcNow;
    }
}
