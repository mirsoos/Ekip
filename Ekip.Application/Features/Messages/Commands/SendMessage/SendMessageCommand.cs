using Ekip.Application.DTOs.Chat;
using MediatR;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public long ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
    }
}
