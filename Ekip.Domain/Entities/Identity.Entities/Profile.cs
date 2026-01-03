using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class Profile : BaseEntity
    {  
        public User UserDetails { get; private set; }
        public double? Score { get; private set; }
        public int Experience { get; private set; }
        public string AvatarUrl { get; private set; }
        private readonly List<User> _userContacts = new();
        public IReadOnlyCollection<User> UserContacts => _userContacts.AsReadOnly();
        public Profile(User userDetails)    
        {
            if (userDetails == null)
                throw new Exception("userDetails Not Found");
            UserDetails = userDetails;
            Score = null;
            Experience = 0;
        }
        public void AddContact(User userContacts)
        {
            if (userContacts == null)
                throw new Exception("userContacts Must Have Value To Add");

            if (_userContacts.Any(c => c.Id == userContacts.Id))
                throw new Exception("this Contact already Exsit");

            _userContacts.Add(userContacts);
        }
        private Profile() { }
    }
}
