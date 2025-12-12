using Ekip.Application.DTOs.Chat;
using MediatR;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public long ChatRoomRef { get; set; }
        public long SenderRef { get; set; }
        public string MessageContent { get; set; }
        public long? ReplyToMessageRef { get; set; }
    }
}
