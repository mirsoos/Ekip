using Ekip.Domain.Base;

namespace Ekip.Domain.Entities.Messaging
{
    public class ConversationParticipant : BaseEntity , IAggregateRoot , IAuditableEntity
    {
        public Guid ConversationId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? LastReadMessageId { get; private set; }
        public DateTime? LastReadAtUtc { get; private set; }
        public int UnreadCount { get; private set; } = 0;
        public bool IsMuted { get; private set; } = false;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
    }
}
