using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public GenderType Gender { get; private set; }
        public bool IsPremium { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLocked { get; private set; }
        public int Age { get; private set; }
        public string PhoneNumber { get; private set; }
        public Guid ProfileRef { get; private set; }
        private readonly List<UserCredential> _userCredentials = new();
        public IReadOnlyCollection<UserCredential> UserCredentials => _userCredentials.AsReadOnly();


        public User(string firstName , string lastName , string userName , string email , GenderType gender,int age,string phoneNumber)
        {

            if (string.IsNullOrWhiteSpace(firstName))
                throw new Exception("firstName cannot Be Empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("lastName cannot Be Empty");
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exception("userName cannot Be Empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new Exception("PhoneNumber cannot Be Empty");

            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            Gender = gender;
            Age = age;
            PhoneNumber = phoneNumber;
            IsPremium = false;
            IsActive = true;
            IsLocked = false;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                throw new Exception("password cannot Be Empty");

            PasswordHash = passwordHash;
        }

        public void addCredential(UserCredential credential)
        {
            if (UserCredentials.Any(x => x.Equals(credential)))
            {
                throw new Exception("this credential already exist.");
            }

            _userCredentials.Add(credential);
        }

        public void DeActivate() => IsActive = false;

        public void SetProfileId(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new ArgumentException("ProfileId cannot be empty");

            ProfileRef = profileId;
        }
        private User() { }
    }
}
