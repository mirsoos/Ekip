using Ekip.Domain.Base;
using Ekip.Domain.Enums.Support;

namespace Ekip.Domain.Entities.Support
{
    public class Ticket : BaseEntity , IAggregateRoot,IAuditableEntity
    {
        public Guid UserId { get; private set; }
        public string Subject { get; private set; }
        public TicketCategory Category { get; private set; }
        public TicketPriority Priority { get; private set; } = TicketPriority.Low;
        public TicketStatus Status { get; private set; } = TicketStatus.Open;
        public Guid? AssignedAdminId { get; private set; }
        public DateTime? ClosedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        private readonly List<TicketMessage> _ticketMessages = [];
        public IReadOnlyCollection<TicketMessage> TicketMessages => _ticketMessages.AsReadOnly();
    }
}
