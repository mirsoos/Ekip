using Ekip.Application.DTOs;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities;
using MediatR;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand,MessageDto>
    {
        private readonly IMessageWriteRepository _writeMessage;
        public readonly IMessageReadRepository _readMessage;
        public SendMessageCommandHandler(IMessageWriteRepository writeMessage,IMessageReadRepository readMessage) 
            {
                _writeMessage = writeMessage;
                _readMessage = readMessage;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request ,CancellationToken cancellationToken)
        {
            var chatRoom = await _readMessage.()

            var message = new Message(chatRoom,request.SenderId,request.MessageContent);

            var messages = await _writeMessage.AddAsync(message, cancellationToken);

            return new MessageDto
            {
                Id = message.Id,
                IsDeleted = message.IsDeleted,
                IsEdited = message.IsEdited,
                MessageContent = message.MessageContent,
                SenderId = message.SenderId,
                SentAt = message.SentAt,
                Type = message.Type,
                ChatRoomId = message.ChatRoomId.Id

            };

        }
    }
}
