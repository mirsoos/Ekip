using Ekip.Domain.Enums.Chat.Enums;

namespace Ekip.Application.DTOs.Chat
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ChatRoomRef { get; set; }
        public Guid SenderRef { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public bool IsReadByMe { get; set; }
        public bool IsPinned { get; set; }
        public Guid? ReplyToMessageRef { get; set; }
    }
}
