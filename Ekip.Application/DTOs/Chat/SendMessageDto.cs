using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        public long ChatRoomRef { get; set; }
        public long SenderRef { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
    }
}
