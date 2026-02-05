using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.Entities.Requests.Entities
{
    public class RequestAssignment : BaseEntity
    {
        public AssignmentStatus Status { get; private set; }
        public Guid SenderRef { get; private set; }
        public string? Description { get; private set; }
        public DateTime ActionDate { get; set; }

        public RequestAssignment(Guid senderRef, string description , AssignmentStatus status)
        {
            SenderRef = senderRef;
            Status = status;
            Description = description;
        }

        public void Accept()
        {
            if (Status != AssignmentStatus.Pending)
                throw new Exception("Request is Not in Pending State");
            Status = AssignmentStatus.Accepted;
            ActionDate = DateTime.UtcNow;
        }

        public void Decline()
        {
            if (Status != AssignmentStatus.Pending)
                throw new Exception("Request is Not in Pending State");
            Status = AssignmentStatus.Declined;
            ActionDate = DateTime.UtcNow;
        }

        private RequestAssignment() { }
    }
}
