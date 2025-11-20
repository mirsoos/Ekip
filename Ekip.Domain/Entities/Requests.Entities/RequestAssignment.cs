using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Domain.Entities.Requests.Entities
{
    public class RequestAssignment : BaseEntitiy
    {
        public Profile Member { get; private set; }
        public Request Request { get; private set; }

        public RequestAssignment(Request request , Profile member)
        {
            if (request == null)
                throw new Exception("the Request Not Found");
            if (member == null)
                throw new Exception("an Assignment need an Member to Assign to this Request");

            Member = member;
            Request = request;
        }
        private RequestAssignment() { }
    }
}
