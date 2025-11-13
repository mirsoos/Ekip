using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.ValueObjects;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class User : BaseEntitiy
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public bool Gender { get; private set; }
<<<<<<< Updated upstream:Ekip.Domain/Entities/Identity.Entities/User.cs
        public double Rating { get; private set; }
=======
        public DateTime CreateDate { get; private set; }
>>>>>>> Stashed changes:Ekip.Domain/Entities/User.cs
        public bool IsPremium { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLocked { get; private set; }
        public Profile Profile { get; private set; }
        private readonly List<UserCredential> _userCredentials = new();
        public IReadOnlyCollection<UserCredential> UserCredentials => _userCredentials.AsReadOnly();


        public User(string firstName , string lastName , string userName , string email , bool gender)
        {

            if (string.IsNullOrEmpty(firstName))
                throw new Exception("firstName cannot Be Empty");
            if (string.IsNullOrEmpty(lastName))
                throw new Exception("lastName cannot Be Empty");
            if (string.IsNullOrEmpty(userName))
                throw new Exception("userName cannot Be Empty");

            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            Gender = gender;
            CreateDate = DateTime.UtcNow;
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

        private User() { }
    }
}
