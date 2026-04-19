using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record AssignmentDecisionMadeEvent
    {
        public Guid AssignmentRef { get; init; }
        public AssignmentStatus NewStatus { get; init; }
    }
}
