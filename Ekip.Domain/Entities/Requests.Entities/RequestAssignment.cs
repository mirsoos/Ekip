using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.Entities.Identity.Entities;

namespace Ekip.Domain.Entities.Requests.Entities
{
    public class RequestAssignment : BaseEntitiy
    {
        public Profile Applicant { get; private set; }
        public Request Request { get; private set; }

        public RequestAssignment(Request request , Profile applicant)
        {
            if (request == null)
                throw new Exception("the Request Not Found");
            if (applicant == null)
                throw new Exception("an Assignment need an Applicant to Assign to this Request");

            Applicant = applicant;
            Request = request;
        }
        private RequestAssignment() { }
    }
}
