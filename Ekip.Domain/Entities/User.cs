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
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public DateTime CreatDate { get; set; }
        public double Rating { get; set; }
        public bool IsPremium { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public List<UserCredential> UserCredentials { get; private set; }


        public void addCredential(UserCredential credential)
        {
            if (UserCredentials.Any(x => x.Equals(UserCredentials)))
            {
                throw new Exception("this credential already exist.");
            }

            UserCredentials.Add(credential);
        }
        public void DeActivate() => IsActive = false;
    }
}
