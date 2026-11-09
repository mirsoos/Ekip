using Ekip.Domain.Base;
using Ekip.Domain.Enums.User;
using Ekip.Domain.ValueObjects.User;

namespace Ekip.Domain.Entities.User
{
    public class UserProfile : Entity , IAuditableEntity
    {
        public Guid UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Gender UserGender { get; private set; }
        public DateOnly BirthDate { get; private set; }
        public Guid? CityId { get; private set; }
        public Guid? AvatarAssetId { get; private set; }
        public Guid? BannerAssetId { get; private set; }
        public string? Bio { get; private set; }
        public string? JobTitle { get; private set; }
        public PersonalMbtiTypes MbtiTypes { get; private set; }
        public MoodWidget? MoodWidget { get; private set; }
        public bool IsSearchable { get; private set; } = true;
        public bool ShowContacts { get; private set; } = true;
        public bool ShowLastSeen { get; private set; } = true;
        public DateTime? LastSeenAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        private readonly List<SocialLink> _socialLinks = [];
        public IReadOnlyCollection<SocialLink> SocialLinks => _socialLinks.AsReadOnly();
        private readonly List<string> _interests = [];
        public IReadOnlyCollection<string> Interests => _interests.AsReadOnly();
    }
}
