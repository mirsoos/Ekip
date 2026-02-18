using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class Profile : BaseEntity
    {  
        public Guid UserRef { get; private set; }
        public double? Score { get; private set; }
        public int Experience { get; private set; }
        public string? AvatarUrl { get; private set; }

        public VerificationLevel VerificationLevel { get; private set; }
        public PhotoEvidence PhotoEvidence { get; private set; }
        public IdentityEvidence IdentityEvidence { get; set; }

        private readonly List<Guid> _userContacts = new();
        public IReadOnlyCollection<Guid> UserContacts => _userContacts.AsReadOnly();
        public Profile(Guid userRef)    
        {
            if (userRef == Guid.Empty)
                throw new Exception("User Not Found");
            UserRef = userRef;
            Score = null;
            Experience = 0;
            VerificationLevel = VerificationLevel.None;
        }
        public void AddContact(Guid userRef)
        {
            if (userRef == Guid.Empty)
                throw new Exception("User Not Found.");

            if (_userContacts.Any(c => c == userRef))
                throw new Exception("this Contact already Exsit");

            _userContacts.Add(userRef);
        }

        public void SetAvatar(string avatarUrl)
        {   
            if (string.IsNullOrWhiteSpace(avatarUrl))
                throw new Exception("Avatar Url Not Found.");
            AvatarUrl = avatarUrl;
        }

        public void UpdateVerificationPhoto(Guid referenceId, string provider,string capturedPhotoUrl)
        {
            if (VerificationLevel == VerificationLevel.FullyVerified)
                return;

            this.PhotoEvidence = new PhotoEvidence(referenceId, provider , capturedPhotoUrl);

            if (this.VerificationLevel == VerificationLevel.IdentityVerified)
                this.VerificationLevel = VerificationLevel.FullyVerified;
            else
                this.VerificationLevel = VerificationLevel.PhotoVerified;
        }

        private Profile() { }
    }
}
