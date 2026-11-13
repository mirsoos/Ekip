using Ekip.Domain.Base;
using Ekip.Domain.Enums.Messaging;
using Ekip.Domain.ValueObjects.Messaging;

namespace Ekip.Domain.Entities.Messaging
{
    public class Conversation : BaseEntity ,IAggregateRoot , IAuditableEntity,ISoftDeletable
    {
        public ConversationType ConversationType { get; private set; }
        public Guid? EkipId { get; private set; }
        public Guid? ParticipantOneId { get; private set; }
        public Guid? ParticipantTwoId { get; private set; }
        public ChatTheme? Theme { get; private set; }
        public Guid? LastMessageId { get; private set; }
        public string? LastMessagePreview { get; private set; }
        public DateTime? LastMessageAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
    }
}
