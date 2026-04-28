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
        public Email Email { get; private set; }
        public GenderType Gender { get; private set; }
        public bool IsPremium { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLocked { get; private set; }
        public int Age { get; private set; }
        public string PhoneNumber { get; private set; }
        public Guid ProfileRef { get; private set; }
        private readonly List<UserCredential> _userCredentials = new();
        public IReadOnlyCollection<UserCredential> UserCredentials => _userCredentials.AsReadOnly();


        public User(string firstName , string lastName , string userName , Email email , GenderType gender,int age,string phoneNumber)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetUserName(userName);
            SetEmail(email);
            SetAge(age);
            SetPhoneNumber(phoneNumber);
            Gender = gender;
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

        public void AddCredential(UserCredential credential)
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

        public void SetFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new Exception("FirstName connot be Empty.");

            FirstName = firstName;
        }
        public void SetLastName(string lastName)
        {
            if(string.IsNullOrWhiteSpace(lastName))
                throw new Exception("LastName connot be Empty.");

            LastName = lastName;
        }
        public void SetUserName(string userName)
        {
            if(string.IsNullOrWhiteSpace(userName))
                throw new Exception("UserName connot be Empty.");

            UserName = userName;
        }
        public void SetEmail(Email email)
        {
            Email = email;
        }
        public void SetAge(int age)
        {
            if (age < 1 || age > 120)
                throw new Exception("Invalid age");
            Age = age;
        }
        public void SetPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length != 11)
                throw new Exception("Phone Number is not Valid.");
            PhoneNumber = phoneNumber;
        }
        private User() { }
    }
}
