using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Chat;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand,MessageDto>
    {
        private readonly IMessageWriteRepository _messageWrite;
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IEventPublisher _eventPublisher;
        public SendMessageCommandHandler(IMessageWriteRepository writeMessage,IChatRoomWriteRepository chatRoomWrite , IEventPublisher eventPublisher) 
            {
                _messageWrite = writeMessage;
                _chatRoomWrite = chatRoomWrite;
                _eventPublisher = eventPublisher;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request ,CancellationToken cancellationToken)
        {

            var chatRoom = await _chatRoomWrite.GetByIdAsync(request.ChatRoomRef, cancellationToken);

            if (chatRoom == null)
                throw new Exception("ChatRoom Not Found.");

            var message = chatRoom.CreateMessage(request.SenderRef , request.MessageContent , request.ReplyToMessageRef);

            var savedMessage = await _messageWrite.AddAsync(message, cancellationToken);

            await _eventPublisher.Publish(new MessageCreatedEvent
            {
                Id = savedMessage.Id,
                SenderRef = savedMessage.SenderRef,
                ChatRoomRef = savedMessage.ChatRoomRef,
                MessageContent = savedMessage.MessageContent,
                SentAt = savedMessage.SentAt,
                Type= savedMessage.Type,
                ReplyToMessageRef = savedMessage.ReplyToMessageRef,
            },cancellationToken);

            return new MessageDto
            {
                Id = savedMessage.Id,
                IsDeleted = savedMessage.IsDeleted,
                IsEdited = savedMessage.IsEdited,
                MessageContent = savedMessage.MessageContent,
                SenderRef = savedMessage.SenderRef,
                SentAt = savedMessage.SentAt,
                Type = savedMessage.Type,
                ChatRoomRef = savedMessage.ChatRoomRef
                
            };

        }
    }
}
