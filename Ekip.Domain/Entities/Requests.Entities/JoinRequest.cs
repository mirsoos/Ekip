using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Entities.Identity;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.Entities.Requests.Entities
{
    /// <summary>
    /// درخواست عضویت در اکیپ
    /// </summary>
    public class JoinRequest : BaseEntitiy
    {
        public JoinRequestStatus Status { get; private set; }

        public Profile Member { get; private set; }
        public Request Request { get; private set; }
        public string? Description { get; private set; }

        public JoinRequest(Request request,Profile member, string? description)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            Request = request;
            Member = member;
            Status = JoinRequestStatus.Pending;
            Description = description;
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
