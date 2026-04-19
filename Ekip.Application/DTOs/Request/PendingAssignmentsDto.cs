
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class PendingAssignmentsDto
    {
        public Guid AssignmentRef { get; set; }
        public Guid RequestRef { get; set; }
        public string RequestTitle { get; set; }
        public string CreatorName { get; set; }
        public DateTime RequestDateTime { get; set; }
        public AssignmentStatus Status { get; set; }
        public DateTime AppliedDate { get; set; }
        public string? Description { get; set; }
    }
}
