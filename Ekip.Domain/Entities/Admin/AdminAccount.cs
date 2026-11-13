using Ekip.Domain.Base;
using Ekip.Domain.Enums.Admin;

namespace Ekip.Domain.Entities.Admin
{
    public class AdminAccount : BaseEntity , IAggregateRoot,IAuditableEntity,ISoftDeletable
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PasswordHash { get; private set; }
        public AdminRole Role { get; private set; }
        public string? TotpSecret { get; private set; }
        public bool IsTwoFactorActivated { get; private set; }
        public int FailedLoginAttempts { get; private set; } = 0;
        public DateTime? LockedUntilUtc { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime? LastLoginAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
        private readonly List<string> _allowedIps = [];
        public IReadOnlyCollection<string> AllowedIps => _allowedIps.AsReadOnly();
        private readonly List<string> _permissions = [];
        public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();
        public bool IsLockedOutAt(DateTime utcNow) => LockedUntilUtc.HasValue && LockedUntilUtc.Value > utcNow;

    }
}
