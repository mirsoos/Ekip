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
        public long Id { get; init; }
        public bool IsDeleted { get; init; }
        public long ChatRoomRef { get;  init; }
        public long SenderRef { get;  init; }
        public string MessageContent { get;  init; }
        public DateTime SentAt { get;  init; }
        public bool IsEdited { get;  init; }
        private readonly List<long> _seenBy = new();
        public long? ReplyToMessageRef { get;  init; }
        public MessageType Type { get;  init; }
    }
}
