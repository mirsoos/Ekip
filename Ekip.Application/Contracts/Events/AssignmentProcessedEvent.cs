
using Ekip.Domain.Enums.Requests.Enums;

namespace Ekip.Application.Contracts.Events
{
    public record class AssignmentProcessedEvent
    {
        public Guid Id { get; init; }
        public Guid RequestRef { get; init; }
        public DateTime CreateDate { get; init; }
        public AssignmentStatus AssignmentStatus { get; init; }
        public Guid SenderRef { get; init; }
        public string? Description { get; init; }
        public DateTime ActionDate { get; init; }
        public RequestStatus RequestStatus { get; init; }

    }
}
