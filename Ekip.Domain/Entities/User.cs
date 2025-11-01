using Ekip.Domain.Enums;
using Ekip.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities
{
    public class User
    {
        public int ID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public bool Gender { get; private set; }
        public DateTime CreateDate { get; private set; }
        public double Rating { get; private set; }
        public bool IsPremium { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLocked { get; private set; }
        public List<UserCredential> UserCredentials { get; private set; }


        public void addCredential(UserCredential credential)
        {
            if (UserCredentials.Any(x => x.Equals(credential)))
            {
                throw new Exception("this credential already exist.");
            }

            UserCredentials.Add(credential);
        }
        public void DeActivate() => IsActive = false;
    }
}
