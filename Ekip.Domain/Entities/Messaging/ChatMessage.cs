using Ekip.Domain.Base;
using Ekip.Domain.Enums.Messaging;
using Ekip.Domain.ValueObjects.Ekip;
using Ekip.Domain.ValueObjects.Messaging;

namespace Ekip.Domain.Entities.Messaging
{
    public class ChatMessage : BaseEntity, IAggregateRoot, IAuditableEntity, ISoftDeletable
    {
        public Guid ConversationId { get; private set; }
        public Guid SenderId { get; private set; }
        public Guid? ReplyToMessageId { get; private set; }
        public ChatMessageType Type { get; private set; }
        public string? ContentText { get; private set; }
        public ChatAttachment? Attachment { get; private set; }
        public GeoLocation? Location { get; private set; }
        public Guid? ReferencedEntityId { get; private set; }
        public bool IsEdited { get; private set; } = false;
        public bool IsPinned { get; private set; } = false;
        public Guid IdempotencyKey { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAtUtc { get; private set; }
    }
}