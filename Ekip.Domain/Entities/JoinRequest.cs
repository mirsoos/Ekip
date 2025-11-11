
using Ekip.Domain.Enums;

namespace Ekip.Domain.Entities
{
    public class JoinRequest
    {
        public long Id { get; private set; }

        public JoinRequestStatus Status { get; private set; }

        public Profile Applicant { get; private set; }

        public Request Request { get; private set; }

        public JoinRequest(Request request,Profile applicant)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (applicant == null)
                throw new ArgumentNullException(nameof(applicant));

            Request = request;
            Applicant = applicant;
            Status = JoinRequestStatus.Pending;
        }

        public void Accept()
        {
            if (Status != JoinRequestStatus.Pending)
                throw new Exception("Request is Not in Pending State");
            Status = JoinRequestStatus.Accepted;
        }

        public void Decline()
        {
            if (Status != JoinRequestStatus.Pending)
                throw new Exception("Request is Not in Pending State");
            Status = JoinRequestStatus.Declined;
        }

        private JoinRequest() { }
    }
}
