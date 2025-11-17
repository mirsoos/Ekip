using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        public long ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
    }
}
