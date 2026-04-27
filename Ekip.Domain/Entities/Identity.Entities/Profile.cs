using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class Profile
    {
        public Guid Id { get; private set; }
        public Guid UserRef { get; private set; }
        public int Experience { get; private set; }
        public string? AvatarUrl { get; private set; }
        public string? Bio { get; private set; }
        public Guid RowVersion { get; private set; }
        public double? Score { get; private set; }
        public int TotalScoreSum { get; private set; }
        public int TotalScoreCount { get; private set; }

        public VerificationLevel VerificationLevel { get; private set; }
        public PhotoEvidence PhotoEvidence { get; private set; }
        public IdentityEvidence IdentityEvidence { get; set; }

        private readonly List<Guid> _userContacts = new();
        public IReadOnlyCollection<Guid> UserContacts => _userContacts.AsReadOnly();
        public Profile(Guid userRef)    
        {
            if (userRef == Guid.Empty)
                throw new Exception("User Not Found");
            Id = Guid.NewGuid();
            RowVersion = Guid.NewGuid();
            UserRef = userRef;
            Score = null;
            Experience = 0;
            VerificationLevel = VerificationLevel.None;
            TotalScoreCount = 0;
            TotalScoreSum = 0;
        }
        public void AddContact(Guid userRef)
        {
            if (userRef == Guid.Empty)
                throw new Exception("User Not Found.");

            if (_userContacts.Any(c => c == userRef))
                throw new Exception("this Contact already Exsit");

            _userContacts.Add(userRef);
        }

        private void IncrementVersion()
        {
            RowVersion = Guid.NewGuid();
        }

        public void SetAvatar(string avatarUrl)
        {   
            if (string.IsNullOrEmpty(avatarUrl))
                throw new Exception("Avatar Url Not Found.");
            AvatarUrl = avatarUrl;
        }

        public void SetBio(string? bio)
        {
            Bio = bio;
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
