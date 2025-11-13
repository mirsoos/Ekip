using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Requests.Entities;

namespace Ekip.Domain.Entities.Identity.Entities
{
    public class Profile : BaseEntitiy
    {  
        public User UserDetails { get; private set; }
        public double? Rating { get; private set; }
        public int Level { get; private set; }
        private readonly List<User> _userContacts = new();
        private readonly List<Request> _requests = new();
        private readonly List<RequestAssignment> _requestAssignments = new();
        public IReadOnlyCollection<User> UserContacts => _userContacts.AsReadOnly();
        public IReadOnlyCollection<Request> Requesters => _requests.AsReadOnly();
        public IReadOnlyCollection<RequestAssignment> RequestAssignments => _requestAssignments.AsReadOnly();

        public Profile(User userDetails)    
        {
            if (userDetails == null)
                throw new Exception("userDetails Not Found");
            UserDetails = userDetails;
            Rating = null;
        }
        public void AddContact(User userContacts)
        {
            if (userContacts == null)
                throw new Exception("userContacts Must Have Value To Add");

            if (_userContacts.Any(c => c.ID == userContacts.ID))
                throw new Exception("this Contact already Exsit");

            _userContacts.Add(userContacts);
        }

        public Request CreateNewRequest(string title , int reqireAssignment)
        {
            var newRequest = new Request(this,title,reqireAssignment);
            _requests.Add(newRequest);
            return newRequest;
        }
        private Profile() { }
    }
}
