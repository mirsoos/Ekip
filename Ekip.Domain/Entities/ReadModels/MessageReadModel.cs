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
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public Guid ChatRoomRef { get; set; }
        public Guid SenderRef { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        private readonly List<Guid> _seenBy = new();
        public Guid? ReplyToMessageRef { get; set; }
        public MessageType Type { get; set; }
    }
}
