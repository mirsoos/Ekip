using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Domain.Entities.ReadModels
{
    public class RequestAssignmentReadModel
    {
        public long Id { get; set; }
        public long RequestRef { get; set; }
        public DateTime CreateDate { get; set; }
        public AssignmentStatus Status { get; set; }
        public long SenderRef { get; set; }
        public string? Description { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
