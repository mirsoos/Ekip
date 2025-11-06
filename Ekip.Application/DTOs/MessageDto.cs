using Ekip.Domain.Enums;

namespace Ekip.Application.DTOs
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
    }
}
