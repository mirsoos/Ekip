using Ekip.Application.DTOs.Chat;
using MediatR;

namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public Guid ChatRoomRef { get; set; }
        public Guid SenderRef { get; set; }
        public string MessageContent { get; set; }
        public Guid? ReplyToMessageRef { get; set; }
    }
}
