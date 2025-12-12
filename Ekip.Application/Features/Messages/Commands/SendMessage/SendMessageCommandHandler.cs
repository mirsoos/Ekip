using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Chat;
using Ekip.Application.Interfaces;
using MassTransit;
using MediatR;
using MessageEntity = Ekip.Domain.Entities.Chat.Entites.Message;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand,MessageDto>
    {
        private readonly IMessageWriteRepository _writeMessage;
        private readonly IChatRoomReadRepository _chatRoomRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public SendMessageCommandHandler(IMessageWriteRepository writeMessage,IChatRoomReadRepository chatRoomRepository , IPublishEndpoint publishEndpoint) 
            {
                _writeMessage = writeMessage;
                _chatRoomRepository = chatRoomRepository;
                _publishEndpoint = publishEndpoint;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request ,CancellationToken cancellationToken)
        {

            var chatRoom = await _chatRoomRepository.GetByIdAsync(request.ChatRoomRef, cancellationToken);

            if (chatRoom == null)
                throw new Exception("ChatRoom Not Found");

            var message = new MessageEntity(request.ChatRoomRef,request.SenderRef,request.MessageContent,request.ReplyToMessageRef);

            var savedMessage = await _writeMessage.AddAsync(message, cancellationToken);

            await _publishEndpoint.Publish(new MessageCreatedEvent
            {
                Id = savedMessage.Id,
                SenderRef = savedMessage.SenderRef,
                ChatRoomRef = savedMessage.ChatRoomRef,
                MessageContent = savedMessage.MessageContent,
                SentAt = savedMessage.SentAt,
                IsEdited = savedMessage.IsEdited,
                Type= savedMessage.Type,
                ReplyToMessageRef = savedMessage.ReplyToMessageRef,
                IsDeleted = savedMessage.IsDeleted
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
