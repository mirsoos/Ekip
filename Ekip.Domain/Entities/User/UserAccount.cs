using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.Entities.User
{
    public class UserAccount : BaseEntity, IAggregateRoot, IAuditableEntity , ISoftDeletable
    {
        public string Username { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; }
        public UserAccountStatus AccountStatus { get; private set; } = UserAccountStatus.Active;
        public bool TwoFactorEnabled { get; private set; } = false;
        public bool IsFaceVerified { get; private set; } = false;
        public bool IsNationalIdVerified { get; private set; } = false;
        public string? NationalIdHash { get; private set; }
        public string ReferralCode { get; private set; }
        public Guid? ReferredById { get; private set; }
        public bool HasCompletedTour { get; private set; } = false;
        public DateTime TermsAcceptedAt { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
        private readonly List<OAuthProviderLogin> _oauthProviders = [];
        public IReadOnlyCollection<OAuthProviderLogin> OAuthProviders => _oauthProviders.AsReadOnly();
    }
}
