
namespace Ekip.Domain.Entities
{
    public class RequestAssignment
    {
        public long Id { get; private set; }
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
