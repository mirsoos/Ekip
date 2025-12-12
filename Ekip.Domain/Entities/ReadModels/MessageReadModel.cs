using Ekip.Domain.Enums.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities.ReadModels
{
    public class MessageReadModel
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public long ChatRoomRef { get; set; }
        public long SenderRef { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        private readonly List<long> _seenBy = new();
        public long? ReplyToMessageRef { get; set; }
        public MessageType Type { get; set; }
    }
}
