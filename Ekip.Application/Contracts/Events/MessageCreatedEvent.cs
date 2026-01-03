using Ekip.Domain.Enums.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Application.Contracts.Events
{
    public record MessageCreatedEvent
    {
        public Guid Id { get; init; }
        public bool IsDeleted { get; init; }
        public Guid ChatRoomRef { get;  init; }
        public Guid SenderRef { get;  init; }
        public string MessageContent { get;  init; }
        public DateTime SentAt { get;  init; }
        public bool IsEdited { get;  init; }
        private readonly List<Guid> _seenBy = new();
        public Guid? ReplyToMessageRef { get;  init; }
        public MessageType Type { get;  init; }
    }
}
