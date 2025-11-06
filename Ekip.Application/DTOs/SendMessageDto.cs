using Ekip.Domain.Enums;


namespace Ekip.Application.DTOs
{
    public class SendMessageDto
    {
        public long ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
    }
}
