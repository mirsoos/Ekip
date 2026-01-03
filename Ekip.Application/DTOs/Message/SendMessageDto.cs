using Ekip.Domain.Enums.Chat.Enums;


namespace Ekip.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        public Guid ChatRoomRef { get; set; }
        public Guid SenderRef { get; set; }
        public string MessageContent { get; set; }
        public MessageType Type { get; set; }
        public Guid? ReplyToMessageRef { get; set; }
    }
}
