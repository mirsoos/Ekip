using Ekip.Domain.Base;
using Ekip.Domain.Enums.Notification;

namespace Ekip.Domain.Entities.Notification
{
    public class Notification : BaseEntity , IAggregateRoot , ICreatedAuditable
    {
        public Guid RecipientId { get; private set; }
        public Guid? ActorId { get; private set; }
        public NotificationType Type { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Guid? ReferenceEntityId { get; private set; }
        public string? DeepLink { get; private set; }
        public bool IsRead { get; private set; } = false;
        public DateTime? ReadAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
