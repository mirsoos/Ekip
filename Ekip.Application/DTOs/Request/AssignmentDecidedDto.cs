using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.DTOs.Request
{
    public class AssignmentDecisionDto
    {
        public Guid OwnerId { get; set; }
        public Guid SenderRef { get; set; }
        public Guid AssignmentRef { get; set; }
        public AssignmentDecision Decision { get; set; }
    }
}
