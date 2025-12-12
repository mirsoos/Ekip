using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record RequestAssignmentCreatedEvent
    {
        public long Id { get; init; }
        public long RequestRef { get; init; }
        public DateTime CreateDate { get; init; }
        public AssignmentStatus Status { get; init; }
        public long SenderRef { get; init; }
        public string? Description { get; init; }
        public DateTime ActionDate { get; init; }
    }
}
